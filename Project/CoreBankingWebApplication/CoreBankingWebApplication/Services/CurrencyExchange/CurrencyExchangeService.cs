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

/// <summary>
/// Service dedicated for exchange currency operation between two bank accounts and two reserves
/// from a relational database.
/// </summary>
public class CurrencyExchangeService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrencyRateService _currencyRateService;

    /// <summary>
    /// The constructor for the service
    /// </summary>
    /// <param name="dbContext">The database context</param>
    /// <param name="currencyRateService">A specific implementation for calculates the rate of each currency conversion</param>
    public CurrencyExchangeService(ApplicationDbContext dbContext, ICurrencyRateService currencyRateService)
    {
        _dbContext = dbContext;
        _currencyRateService = currencyRateService;
     
    }




    /// <summary>
    /// Executes asynchronously the exchange service. This services is processed if all steps are
    /// completed without errors. In error scenarios the database is reverted in the previous state.
    /// 1. Context rules:
    ///     - Transaction between account context <see cref="Models.Domain.AccountBalanceTransactionContext"/>
    ///     - Bank reserves in exchange context <see cref="Models.Domain.ExchangeReserveOperationContext"/>
    /// 2. Bank currencies must be different
    /// 3. 
    /// </summary>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    public async Task<CurrencyExchangeResult> ExchangeAsync(CurrencyExchangeRequestDto requestDto) // 39
    {
        CurrencyExchangeDetailsResponseDto responseDto;

        // Fast validations (without database)
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
            var bankTransactionContextResult = AccountBalanceTransactionContext.CreateValidContext(senderBankAccount, receiverBankAccount, requestDto.Amount);

            if (bankTransactionContextResult.IsFailure) return MapToExchangeServiceResultError(bankTransactionContextResult.Error);
            var banksTransactionContext = bankTransactionContextResult.Value;

            //Ensures that both are using differents currencies (specific service rule)
            if (banksTransactionContext.Sender.CurrencyId == banksTransactionContext.Receiver.CurrencyId)
            {
                return CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.SameCurrencyType, "Currency type must be different");
            }

            // Get rate
          
            var currencyRateServiceResult = await _currencyRateService.GetConversionRateAsync(
                fromCurrencyCode: banksTransactionContext.Sender.Currency.Code,
                toCurrencyCode: banksTransactionContext.Receiver.Currency.Code
            );

            if (currencyRateServiceResult.IsFailure)
            {
                return CurrencyExchangeResult.
                    Fail(CurrencyExchangeServiceError.ConversionFailed, "Internal implementation for conversion failed");
            }

            // Calculates the exchange amount details
            var amountDetails = new ExchangeAmountDetails(
                banksTransactionContext.Sender.Currency.Code,
                requestDto.Amount,
                banksTransactionContext.Receiver.Currency.Code,
                currencyRateServiceResult.Value,
                0.0m
            );

            var bankReservesRecords = await _dbContext.CurrencyExchangeReserves
                .Include(e => e.Currency) // Charge the navigation property for more later
                .Where(e => e.CurrencyId == banksTransactionContext.Sender.Currency.CurrencyId || e.CurrencyId == banksTransactionContext.Receiver.Currency.CurrencyId)
                .ToDictionaryAsync(k => k.CurrencyId);

            var debitedReserve = bankReservesRecords.GetValueOrDefault(banksTransactionContext.Sender.Currency.CurrencyId);
            var accreditedReserve = bankReservesRecords.GetValueOrDefault(banksTransactionContext.Receiver.Currency.CurrencyId);;
            
            // Ensures that rate convertion service has rate conversion for defined currency
            // Ensures that debited currency exchange reserve has enough balance for the operation

            var bilateralExchangeReserveCtxResult = ExchangeReserveOperationContext.CreateValidContext(accreditedReserve, debitedReserve, amountDetails.NetAmount);
            if (bilateralExchangeReserveCtxResult.IsFailure)
            {
                return MapToExchangeServiceResultError(bilateralExchangeReserveCtxResult.Error);
            }
            
            var bilateralExchangeReserveCtx = bilateralExchangeReserveCtxResult.Value;

            // Transactions operations occurs
            StartBalanceTransactions(banksTransactionContext, bilateralExchangeReserveCtx);

            // Generates audit table for exchange transactions
            var auditTable = CreateAuditRecord(banksTransactionContext, bilateralExchangeReserveCtx, amountDetails);


            await _dbContext.CurrencyExchanges.AddAsync(auditTable);

            await _dbContext.SaveChangesAsync();

            // Prepares response dto
            responseDto = MapToResponseDto(auditTable);


            await transaction.CommitAsync();

        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        return CurrencyExchangeResult.Success(responseDto);
    } // 192



    private void StartBalanceTransactions(AccountBalanceTransactionContext accountsContext, ExchangeReserveOperationContext reservesContext)
    {
        // Acredites to reserve and debited from bank account
        accountsContext.Sender.Balance -= accountsContext.DebitedAmount;
        reservesContext.AccreditedReserve.Balance += accountsContext.DebitedAmount;

        // Debites from reserve and acredites to receiver bank account
        accountsContext.Receiver.Balance += reservesContext.DebitedAmount;
        reservesContext.DebitedReserve.Balance -= reservesContext.DebitedAmount;
    }

    private CurrencyExchangeDetailsResponseDto MapToResponseDto(CurrencyExchange entity)
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
    /// Creates an object that represents an audit table for Currency Exchange :n
    /// </summary>
    /// <param name="banksTransactionContext">A valid bilateral bank transaction context</param>
    /// <param name="reserveExchangeContext">A valid reserve exchange context</param>
    /// <param name="exchangeAAAmounts">Exchange amount details</param>
    /// <returns>A CurrencyExchange object that represents the details of the exchange operation</returns>
    private CurrencyExchange CreateAuditRecord(
        AccountBalanceTransactionContext banksTransactionContext,
        ExchangeReserveOperationContext reserveExchangeContext,
        ExchangeAmountDetails exchangeAmounts)
    {
        return new CurrencyExchange()
        {
            SenderBankAccountId = banksTransactionContext.Sender.BankAccountId,
            SenderBankAccount = banksTransactionContext.Sender,
            ReceiverBankAccountId = banksTransactionContext.Receiver.BankAccountId,
            ReceiverBankAccount = banksTransactionContext.Receiver,
            DebitedExchangeReserveId = reserveExchangeContext.DebitedReserve.CurrencyExchangeReserveId,
            AccreditedExchangeReserveId = reserveExchangeContext.AccreditedReserve.CurrencyExchangeReserveId,
            SourceAmount = exchangeAmounts.SourceAmount,
            BankUnitPrice = exchangeAmounts.BankUnitPrice,
            GrossAmount = exchangeAmounts.GrossAmount,
            BankDiscountInPercentage = exchangeAmounts.BankDiscountInPercentage,
            BankDiscount = exchangeAmounts.BankDiscount,
            NetAmount = exchangeAmounts.NetAmount,
            CreateAt = DateTime.UtcNow
        };
    }
    // For BankTransactionContextError
    private CurrencyExchangeResult MapToExchangeServiceResultError(Error<AccountBalanceTransactionContextError> error)
    {
        return error.Code switch
        {
            AccountBalanceTransactionContextError.ReceiverBankAccountNotFound =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.ReceiverBankAccountNotFound, error.Message),

            AccountBalanceTransactionContextError.SenderBankAccountNotFound =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.SenderBankAccountNotFound, error.Message),

            AccountBalanceTransactionContextError.SameBankAccounts =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.SameBankAccounts, error.Message),

            AccountBalanceTransactionContextError.NegativeOrZeroAmount =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.NegativeOrZeroAmount, error.Message),

            AccountBalanceTransactionContextError.NotEnoughBalance =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.NotEnoughBalance, error.Message),

            _ => throw new InvalidOperationException($"Unmapped error code: {error.Code}")
        };
    }

    // For BilateraExchangeReserveContextError
    private CurrencyExchangeResult MapToExchangeServiceResultError(Error<ExchangeReserveOperationContextError> error)
    {
        return error.Code switch
        {
            ExchangeReserveOperationContextError.AccreditedReserveDoesntExist =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.HasNotReserveForAcceptTheCurrencies, "Has not reserve for accept the currencies"),
            
            ExchangeReserveOperationContextError.DebitedReserveDoesntExist =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.HasNotReserveForDepositTheCurrencies, "Has not reserve for accept the currencies"),
            
            ExchangeReserveOperationContextError.HasNotEnoughBalanceInCurrencyReserve =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.HasNotEnoughBalanceInCurrencyReserve, error.Message),
            
            ExchangeReserveOperationContextError.NegativeOrZeroAmount =>
                CurrencyExchangeResult.Fail(CurrencyExchangeServiceError.NegativeOrZeroAmount, error.Message),

            _ => throw new InvalidOperationException($"Unmapped error code: {error.Code}")
        };
    }


}
