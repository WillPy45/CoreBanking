using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;
//using CoreBankingWebApplication.Migrations;

namespace CoreBankingWebApplication.Services
{   
    using CurrencyExchangeResult = Result<CurrencyExchangeDetailsResponseDto, CurrencyExchangeServiceError>;
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
            4. Ensures that sender and receiver accounts are differents currencies
            5. Ensures that sender has enough money for the transaction
            6. Ensures that currency exchange reserves exists in the database
            7. Ensures that debited currency exchange reserve has enough balance for the operation
            8. Ensures that rate convertion service has rate conversion for defined currency
            9. Transactions operations occurs 
            10. Generates audit table for exchange transactions
            11. Generates response dto
        */

        public async Task<CurrencyExchangeResult> ExchangeAsync(CurrencyExchangeRequestDto requestDto)
        {
            CurrencyExchangeDetailsResponseDto responseDto;
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Ensures that are different accounts
                if (requestDto.ReceiverBankAccountId == requestDto.SenderBankAccountId)
                {
                    return CurrencyExchangeResult
                        .Fail(CurrencyExchangeServiceError.SameBankAccounts, "Is not posible to operates between same bank accounts");
                }

                // Ensures that uses positive amount
                if (requestDto.Amount <= 0)
                {
                    return CurrencyExchangeResult
                        .Fail(CurrencyExchangeServiceError.NegativeOrZeroAmount, "Amount must be positive number");  
                }

                // Ensures that sender and receiver exists
                
                int senderId = requestDto.SenderBankAccountId;
                int receiverId = requestDto.ReceiverBankAccountId;

                var bankAccountsRecords = await _dbContext.BankAccounts
                    .Include(e => e.Currency)
                    .Where(e => e.BankAccountId == senderId || e.BankAccountId == receiverId)
                    .ToListAsync();
                
                var senderBankAccount = bankAccountsRecords.Find(e => e.BankAccountId == senderId);
                var receiverBankAccount = bankAccountsRecords.Find(e => e.BankAccountId == receiverId);

                if (senderBankAccount is null)
                {
                    return CurrencyExchangeResult.Fail(
                        CurrencyExchangeServiceError.SenderBankAccountNotFound, "Sender bank account not found");
                }

                if (receiverBankAccount is null)
                {
                    return CurrencyExchangeResult.Fail(
                        CurrencyExchangeServiceError.ReceiverBankAccountNotFound, "Receiver bank account not found");
                }

                //Ensures that both are using differents currencies
                if (receiverBankAccount.Currency.Code == senderBankAccount.Currency.Code){
                    return CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.SameCurrencyType, "Currency type must be different");
                }

                // Ensures that sender has enough money for the transaction
                if (senderBankAccount.Balance < requestDto.Amount)
                {
                    return CurrencyExchangeResult.Fail(
                        CurrencyExchangeServiceError.NotEnoughBalance, "Sender balance is not enough for this transaction");
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
                    return CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.HasNotReserveForAcceptTheCurrencies,
                         $"Bank reserves cannot accept {senderBankAccount.Currency.Code}");
                }

                if (debitedReserve is null)
                {
                    /*if (receiverBankAccount.Currency is null)
                    {
                        throw new Exception("navigation property is null, implementation error"); 
                    }*/ 
                    return CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.HasNotReserveForDepositTheCurrencies,
                        $"Bank reserves cannot deposit {receiverBankAccount.Currency.Code}");
                }


                // Ensures that rate convertion service has rate conversion for defined currency

                if (accreditedReserve.Currency is null) throw new Exception("navigation property is null, implementation error"); 
                if (debitedReserve.Currency is null) throw new Exception("navigation property is null, implementation error"); 

                string fromCurrencyCode = accreditedReserve.Currency.Code;
                string toCurrencyCode = debitedReserve.Currency.Code;

                var currencyRateServiceResult = await _currencyRateService.GetConversionRateAsync(
                    fromCurrencyCode: fromCurrencyCode,
                    toCurrencyCode: toCurrencyCode
                );

                if (currencyRateServiceResult.IsFailure)
                {
                    return CurrencyExchangeResult.
                        Fail(CurrencyExchangeServiceError.ConversionFailed, "Internal implementation for conversion failed");
                }


                // Ensures that debited currency exchange reserve has enough balance for the operation
                
                decimal rate = currencyRateServiceResult.Value;
               
                decimal amount = requestDto.Amount;
                decimal unitPrice = rate != 0 ? 1 / rate : 0;

                // gross amount is the converted amount
                decimal grossAmount = amount * rate;

                if (debitedReserve.Balance < grossAmount)
                {
                    return CurrencyExchangeResult
                        .Fail(CurrencyExchangeServiceError.HasNotEnoughBalanceInCurrencyReserve,
                        $"Bank reserve has not enough balance to deposit in receiver account");
                }


                // Transactions operations occurs 

                // Some administrative discounts (if applies)
                decimal bankDiscountInPercentage = 0.0m;
                decimal bankDiscount = grossAmount * bankDiscountInPercentage;
                decimal netAmount = grossAmount - bankDiscount;

                // Acredites to reserve and debited from bank account
                accreditedReserve.Balance += amount;
                senderBankAccount.Balance -= amount;
              
                // Debites from receiver account and acredites to reserve
              
                debitedReserve.Balance -= netAmount;
                receiverBankAccount.Balance += netAmount;

        

                // Generates audit table for exchange transactions
                var auditTable = CreateAuditRecord(senderBankAccount, receiverBankAccount, debitedReserve, accreditedReserve, amount, grossAmount, unitPrice, bankDiscountInPercentage, bankDiscount, netAmount);


                await _dbContext.CurrencyExchanges.AddAsync(auditTable);

                await _dbContext.SaveChangesAsync();

                // Prepares te response dto
                responseDto = MapToResponseDto(auditTable);

                
                await transaction.CommitAsync();


            }catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return CurrencyExchangeResult.Success(responseDto);
        }

        public CurrencyExchangeDetailsResponseDto MapToResponseDto(CurrencyExchange entity)
        {
            return new()
            {
                CurrencyExchangeId = entity.CurrencyExchangeId,
                SenderBankAccountId = entity.SenderBankAccountId,
                ReceiverBankAccountId = entity.ReceiverBankAccountId,
                FromCurrencyCode = entity.SenderBankAccount.Currency.Code,
                ToCurrencyCode = entity.ReceiverBankAccount.Currency.Code,
                SourceAmount = entity.SourceAmount,
                BankUnitPrice = entity.BankUnitPrice,
                GrossAmount = entity.GrossAmount,
                BankDiscountInPercentage = entity.BankDiscountInPercentage,
                BankDiscount = entity.BankDiscount,
                NetAmount = entity.NetAmount,
                CreateAt = entity.CreateAt     
            };
        }

        /// <summary>
        /// Creates an object that represents a audit table for Currency Exchange
        /// </summary>
        /// <param name="senderBankAccount"></param>
        /// <param name="receiverBankAccount"></param>
        /// <param name="debitedReserve"></param>
        /// <param name="accreditedReserve"></param>
        /// <param name="sourceAmount"></param>
        /// <param name="grossAmount"></param>
        /// <param name="unitPrice"></param>
        /// <param name="bankDiscountInPercentage"></param>
        /// <param name="bankDiscount"></param>
        /// <param name="netAmount"></param>
        /// <returns>CurrencyExchange</returns>
        private CurrencyExchange CreateAuditRecord(
            BankAccount senderBankAccount, BankAccount receiverBankAccount,
            CurrencyExchangeReserve debitedReserve, CurrencyExchangeReserve accreditedReserve,
            decimal sourceAmount, decimal grossAmount, decimal unitPrice, decimal bankDiscountInPercentage,
            decimal bankDiscount, decimal netAmount)
        {
            return new CurrencyExchange()
            {
                SenderBankAccountId = senderBankAccount.BankAccountId,
                ReceiverBankAccountId = receiverBankAccount.BankAccountId,
                DebitedExchangeReserveId = debitedReserve.CurrencyExchangeReserveId,
                AccreditedExchangeReserveId = accreditedReserve.CurrencyExchangeReserveId,
                SourceAmount = sourceAmount,
                BankUnitPrice = unitPrice,
                GrossAmount = grossAmount,
                BankDiscountInPercentage = bankDiscountInPercentage,
                BankDiscount = bankDiscount,
                NetAmount = netAmount,
                CreateAt = DateTime.UtcNow                    
            };
        }
       
       
       
    }
}