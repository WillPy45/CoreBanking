using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Models.Domain;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;
//using CoreBankingWebApplication.Migrations;

namespace CoreBankingWebApplication.Services;

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

        // Fast validations
        // Ensures that are different accounts (fast validation)
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

        //Database context operations
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {


            // Retrieves bank accounts for future validation
            int senderId = requestDto.SenderBankAccountId;
            int receiverId = requestDto.ReceiverBankAccountId;

            var bankAccountsRecords = await _dbContext.BankAccounts
                .Include(e => e.Currency)
                .Where(e => e.BankAccountId == senderId || e.BankAccountId == receiverId)
                .ToListAsync();

            var senderBankAccount = bankAccountsRecords.Find(e => e.BankAccountId == senderId);
            var receiverBankAccount = bankAccountsRecords.Find(e => e.BankAccountId == receiverId);

            // Ensures to follow intrisict transactions rules (Not same account, accounts exists, enough founds
            var bankTransactionContextResult = Models.Domain.AccountBilateralTransactionContext.CreateValidContext(senderBankAccount, receiverBankAccount, requestDto.Amount);

            if (bankTransactionContextResult.IsFailure) return MapToExchangeServiceResultError(bankTransactionContextResult.Error);
            var banksTransactionContext = bankTransactionContextResult.Value;

            //Ensures that both are using differents currencies (specific service rule)
            if (banksTransactionContext.Sender.CurrencyId == banksTransactionContext.Receiver.CurrencyId)
            {
                return CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.SameCurrencyType, "Currency type must be different");
            }

            // Ensures that currency exchange reserve exists in the database
            /*  
                Bank reserve must to have both currency reserves, for example,
                if the bank buy PYG 61000 for 1 USD, the banks must to debited (-) from USD reserve
                and acredited (+) to PYG reserve 
            */
            // From bank perspective
            int currencyDebitedId = banksTransactionContext.Receiver.CurrencyId;
            int currencyAcreditedId = banksTransactionContext.Sender.CurrencyId;

            var bankReservesRecords = await _dbContext.CurrencyExchangeReserves
                .Include(e => e.Currency) // Charge the navigation property for more later
                .Where(e => e.CurrencyId == currencyAcreditedId || e.CurrencyId == currencyDebitedId)
                .ToListAsync();

            var debitedReserve = bankReservesRecords.FirstOrDefault(e => e.CurrencyId == currencyDebitedId);
            var accreditedReserve = bankReservesRecords.FirstOrDefault(e => e.CurrencyId == currencyAcreditedId);

            if (accreditedReserve is null)
            {

                return CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.HasNotReserveForAcceptTheCurrencies,
                     $"Bank reserves cannot accept {banksTransactionContext.Sender.Currency.Code}");
            }

            if (debitedReserve is null)
            {
                return CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.HasNotReserveForDepositTheCurrencies,
                    $"Bank reserves cannot deposit {banksTransactionContext.Receiver.Currency.Code}");
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

            var amountDetails = new ExchangeAmountDetails(
                fromCurrencyCode,
                requestDto.Amount,
                toCurrencyCode,
                currencyRateServiceResult.Value,
                0.0m
            );

            // Ensures that debited currency exchange reserve has enough balance for the operation

            if (debitedReserve.Balance < amountDetails.SourceAmount)
            {
                return CurrencyExchangeResult
                    .Fail(CurrencyExchangeServiceError.HasNotEnoughBalanceInCurrencyReserve,
                    $"Bank reserve has not enough balance to deposit in receiver account");
            }


            // Transactions operations occurs 

            // Acredites to reserve and debited from bank account
            accreditedReserve.Balance += amountDetails.SourceAmount;
            banksTransactionContext.Sender.Balance -= amountDetails.SourceAmount;

            // Debites from receiver account and acredites to reserve

            debitedReserve.Balance -= amountDetails.NetAmount;
            banksTransactionContext.Receiver.Balance += amountDetails.NetAmount;



            // Generates audit table for exchange transactions
            var auditTable = CreateAuditRecord(banksTransactionContext, debitedReserve, accreditedReserve, amountDetails);


            await _dbContext.CurrencyExchanges.AddAsync(auditTable);

            await _dbContext.SaveChangesAsync();

            // Prepares te response dto
            responseDto = MapToResponseDto(auditTable);


            await transaction.CommitAsync();


        }
        catch (Exception)
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
        AccountBilateralTransactionContext banksTransactionContext,
        CurrencyExchangeReserve debitedReserve, CurrencyExchangeReserve accreditedReserve,
        ExchangeAmountDetails exchangeAmounts)
    {
        return new CurrencyExchange()
        {
            SenderBankAccountId = banksTransactionContext.Sender.BankAccountId,
            SenderBankAccount = banksTransactionContext.Sender,
            ReceiverBankAccountId = banksTransactionContext.Receiver.BankAccountId,
            ReceiverBankAccount = banksTransactionContext.Receiver,
            DebitedExchangeReserveId = debitedReserve.CurrencyExchangeReserveId,
            AccreditedExchangeReserveId = accreditedReserve.CurrencyExchangeReserveId,
            SourceAmount = exchangeAmounts.SourceAmount,
            BankUnitPrice = exchangeAmounts.BankUnitPrice,
            GrossAmount = exchangeAmounts.GrossAmount,
            BankDiscountInPercentage = exchangeAmounts.BankDiscountInPercentage,
            BankDiscount = exchangeAmounts.BankDiscount,
            NetAmount = exchangeAmounts.NetAmount,
            CreateAt = DateTime.UtcNow
        };
    }

    public CurrencyExchangeResult MapToExchangeServiceResultError(Error<AccountBilateralTransactionContextError> error)
    {
        return error.Code switch
        {
            AccountBilateralTransactionContextError.ReceiverBankAccountNotFound =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.ReceiverBankAccountNotFound, error.Message),

            AccountBilateralTransactionContextError.SenderBankAccountNotFound =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.SenderBankAccountNotFound, error.Message),

            AccountBilateralTransactionContextError.SameBankAccounts =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.SameBankAccounts, error.Message),

            AccountBilateralTransactionContextError.NegativeOrZeroAmount =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.NegativeOrZeroAmount, error.Message),

            AccountBilateralTransactionContextError.NotEnoughBalance =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.NotEnoughBalance, error.Message),

            _ => throw new InvalidOperationException($"Unmapped error code: {error.Code}")
        };
    }

}
