using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;
using CoreBankingWebApplication.Migrations;

namespace CoreBankingWebApplication.Services
{
    public class CurrencyExchangeService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrencyRateService _currencyRateService;

        public CurrencyExchangeService(ApplicationDbContext dbContext, ICurrencyRateService currencyRateService)
        {
            _dbContext = dbContext;
            _currencyRateService = currencyRateService;
        }

        // Rules:
        /*
            1. Ensures that are different accounts
            2. Ensures that uses positive amount
            3. Ensures that sender and receiver exists
            3. Ensures that sender has enough money for the transaction
            3. Ensures that currency exchange reserves exists in the database
            4. Ensures that debited currency exchange reserve has enough balance for the operation
            5. Ensures that rate convertion service has rate conversion for defined currency
            6. Transactions operations occurs 
            7. Generates audit table for exchange transactions
            8. Generates response dto
        */

        public async Task<Result<CurrencyExchangeDetailsResponseDto>> Exchange(CurrencyExchangeRequestDto requestDto)
        {
            CurrencyExchangeDetailsResponseDto responseDto;
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Ensures that are different accounts
                if (requestDto.ReceiverBankAccountId == requestDto.SenderBankAccountId)
                {
                    return Result<CurrencyExchangeDetailsResponseDto>
                        .Fail(new Error("BankAccount.SameBankAccounts", "Is not posible to operates between same bank accounts"));
                }

                // Ensures that uses positive amount
                if (requestDto.Amount <= 0)
                {
                    return Result<CurrencyExchangeDetailsResponseDto>
                        .Fail(new Error("BankAccount.NegativeOrZeroAmount", "Amount must be positive number"));  
                }

                // Ensures that sender and receiver exists
                
                int senderId = requestDto.SenderBankAccountId;
                int receiverId = requestDto.ReceiverBankAccountId;

                var bankAccountsRecords = await _dbContext.BankAccounts
                    .Include(e => e.Currency)
                    .Where(e => (e.BankAccountId == senderId || e.BankAccountId == receiverId))
                    .ToListAsync();
                
                var senderBankAccount = bankAccountsRecords.Find(e => e.BankAccountId == senderId);
                var receiverBankAccount = bankAccountsRecords.Find(e => e.BankAccountId == receiverId);

                if (senderBankAccount is null)
                {
                    return Result<CurrencyExchangeDetailsResponseDto>.Fail(
                        new Error("BankAccount.SenderAccountNotFound", "Sender bank account not found"));
                }

                if (receiverBankAccount is null)
                {
                    return Result<CurrencyExchangeDetailsResponseDto>.Fail(
                        new Error("BankAccount.ReceiverAccountNotFound", "Receiver bank account not found"));
                }

                // Ensures that sender has enough money for the transaction
                if (senderBankAccount.Balance < requestDto.Amount)
                {
                    return Result<CurrencyExchangeDetailsResponseDto>.Fail(
                        new Error("Transfer.NotEnoughBalance", "Sender balance is not enough for this transaction"));
                }

                // Ensures that currency exchange reserve exists in the database
                /*  
                    Bank reserve must to have both currency reserves, for example,
                    if the bank buy PYG 61000 for 1 USD, the banks must to debited (-) from USD reserve
                    and acredited (+) to PYG reserve 
                */
                // From bank perspective
                int currencyDebitedId = receiverBankAccount.CurrencyId; 
                int currencyAcreditedId = senderBankAccount.CurrencyId;  

                var bankReservesRecords = await _dbContext.CurrencyExchangeReserves
                    .Include(e => e.Currency) // Charge the navigation property for more later
                    .Where(e => e.CurrencyId == currencyAcreditedId || e.CurrencyId == currencyDebitedId)
                    .ToListAsync();
                    
                var debitedReserve = bankReservesRecords.FirstOrDefault(e => e.CurrencyId == currencyDebitedId);
                var accreditedReserve = bankReservesRecords.FirstOrDefault(e => e.CurrencyId == currencyAcreditedId);

                if (accreditedReserve is null)
                {
                    if (senderBankAccount.Currency is null)
                    {
                        throw new Exception("navigation property is null, implementation error"); 
                    }
                    return Result<CurrencyExchangeDetailsResponseDto>
                        .Fail(new Error("BankCurrencyReserve.HasNotReserveForAcceptTheCurrencies",
                         $"Bank reserves cannot accept {senderBankAccount.Currency.Code}"));
                }

                if (debitedReserve is null)
                {
                    if (receiverBankAccount.Currency is null)
                    {
                        throw new Exception("navigation property is null, implementation error"); 
                    }
                    return Result<CurrencyExchangeDetailsResponseDto>
                        .Fail(new Error("BankCurrencyReserve.HasNotReserveForDepositTheCurrencies",
                        $"Bank reserves cannot deposit {receiverBankAccount.Currency.Code}"));
                }


                // Ensures that rate convertion service has rate conversion for defined currency

                if (accreditedReserve.Currency is null) throw new Exception("navigation property is null, implementation error"); 
                if (debitedReserve.Currency is null) throw new Exception("navigation property is null, implementation error"); 

                string fromCurrencyCode = accreditedReserve.Currency.Code;
                string toCurrencyCode = debitedReserve.Currency.Code;

                Result<decimal> currencyRateServiceResult = await _currencyRateService.GetConversionRateAsync(
                    fromCurrencyCode: fromCurrencyCode,
                    toCurrencyCode: toCurrencyCode
                );

                if (currencyRateServiceResult.IsFailure)
                {
                    return Result<CurrencyExchangeDetailsResponseDto>.
                        Fail(new Error("InternalError.ConversionFailed",
                        "Internal implementation for conversion failed"));
                }


                // Ensures that debited currency exchange reserve has enough balance for the operation
                
                decimal rate = currencyRateServiceResult.Value;
               
                decimal amount = requestDto.Amount;
                decimal unitPrice = rate != 0 ? 1 / rate : 0;

                // gross amount is the converted amount
                decimal grossAmount = amount * rate;

                if (debitedReserve.Balance < grossAmount)
                {
                    return Result<CurrencyExchangeDetailsResponseDto>
                        .Fail(new Error("BankCurrencyReserve.HasNotEnoughBalance",
                        $"Bank reserve has not enough balance to deposit in receiver account"));
                }


                // Transactions operations occurs 
                // Acredites to reserve and debited from bank account
                accreditedReserve.Balance += amount;
                senderBankAccount.Balance -= amount;
              
                // Debites from receiver account and acredites to reserve
              
                debitedReserve.Balance -= grossAmount;
                receiverBankAccount.Balance += grossAmount;

                // Some administrative discounts (if applies)
                decimal bankDiscountInPercentage = 0.0m;
                decimal bankDiscount = grossAmount * bankDiscountInPercentage;
                decimal netAmount = grossAmount - bankDiscount;

                // Generates audit table for exchange transactions

                var auditTable = new Models.CurrencyExchange()
                {
                    SenderBankAccountId = senderId,
                    ReceiverBankAccountId = receiverId,
                    DebitedExchangeReserveId = debitedReserve.CurrencyExchangeReserveId,
                    AccreditedExchangeReserveId = accreditedReserve.CurrencyExchangeReserveId,
                    SourceAmount = amount,
                    BankUnitPrice = unitPrice,
                    GrossAmount = grossAmount,
                    BankDiscountInPercentage = bankDiscountInPercentage,
                    BankDiscount = bankDiscount,
                    NetAmount = netAmount,
                    CreateAt = DateTime.UtcNow                    
                };

                await _dbContext.CurrencyExchanges.AddAsync(auditTable);

                await _dbContext.SaveChangesAsync();

                // Prepares te response dto
                responseDto = new()
                {
                    CurrencyExchangeId = auditTable.CurrencyExchangeId,
                    SenderBankAccountId = auditTable.SenderBankAccountId,
                    ReceiverBankAccountId = auditTable.ReceiverBankAccountId,
                    FromCurrencyCode = fromCurrencyCode,
                    ToCurrencyCode = toCurrencyCode,
                    SourceAmount = auditTable.SourceAmount,
                    BankUnitPrice = auditTable.BankUnitPrice,
                    GrossAmount = auditTable.GrossAmount,
                    BankDiscountInPercentage = auditTable.BankDiscountInPercentage,
                    BankDiscount = auditTable.BankDiscount,
                    NetAmount = auditTable.NetAmount,
                    CreateAt = auditTable.CreateAt     
                };

                
                await transaction.CommitAsync();


            }catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return Result<CurrencyExchangeDetailsResponseDto>.Success(responseDto);
        }
       
        
    }
}