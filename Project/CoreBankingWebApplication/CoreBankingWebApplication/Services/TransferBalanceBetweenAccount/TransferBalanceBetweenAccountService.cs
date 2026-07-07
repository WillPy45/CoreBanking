using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Models.Domain;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services;

using GetAllTransactionsResult = Result<ICollection<BalanceTransactionGetResponse>, GetAllTransactionsError>;
using TransferBetweenAccountsResult = Result<string, TransferBalanceBetweenAccountError>;


/// <summary>
/// Service handles transactional balance transfers between two distinct bank accounts,
/// ensuring business rules validation and strict audit logging.
/// </summary>
public class TransferBalanceBetweenAccountService
{
    private readonly ApplicationDbContext _dbContext;

    public TransferBalanceBetweenAccountService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Processes an asynchronous transfer between two bank accounts by their IDs.
    /// </summary>
    /// <remarks>
    /// STEPS:
    /// 1. Validate sender and receiver are distinct to prevent inconsistencies.
    /// 2. Validate that sender sends positive balance
    /// 2. Retrieve both accounts in a single query and ensure existence.
    /// 4. Ensures that currency type are same in both side (Sender and Receiver)
    /// 3. Verify sufficient funds in the sender account.
    /// 4. Update balances and write audit log.
    /// </remarks>
    public async Task<TransferBetweenAccountsResult> TransferAsync(BalanceTransactionDTO transferDto)
    {

        int senderAccountId = transferDto.SenderBankAccountId;
        int receiverAccountId = transferDto.ReceiverBankAccountId;
        decimal transferAmount = transferDto.Amount;

        // Fast validations
        // Prevent self-transfers
        if (senderAccountId == receiverAccountId)
        {
            return TransferBetweenAccountsResult
                .Fail(TransferBalanceBetweenAccountError.SameBankAccount, "Accounts cannot be transferred to yourself");
        }

        // Validate that ammount balance is positive
        if (transferAmount < 0.0m)
        {
            return TransferBetweenAccountsResult
                .Fail(TransferBalanceBetweenAccountError.NegativeOrZeroAmount, "Amount must be positive number");
        }


        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {

            // Fetch both accounts tracked by EF in a single roundtrip to optimize database performance
            var recordsRetrieved = await _dbContext.BankAccounts
                .Where(e => e.BankAccountId == senderAccountId || e.BankAccountId == receiverAccountId)
                .ToListAsync();

            var senderAccount = recordsRetrieved.Find(account => account.BankAccountId == senderAccountId);
            var receiverAccount = recordsRetrieved.Find(account => account.BankAccountId == receiverAccountId);

            //Ensures that accounts exists, both are same currency type, and has enough balance

            var transferContextResult = AccountBalanceTransactionContext.CreateValidContext(senderAccount, receiverAccount, transferAmount);
            if (transferContextResult.IsFailure) return MapToTransferResponse(transferContextResult.Error);
            senderAccount = transferContextResult.Value.Sender;
            receiverAccount = transferContextResult.Value.Receiver;

            // Ensures that both are same currency type (USD to USD, PYG to PYG, NOT USD to PYG)
            if (senderAccount.CurrencyId != receiverAccount.CurrencyId)
            {
                return TransferBetweenAccountsResult.Fail(
                    TransferBalanceBetweenAccountError.CurrencyDiffers, "Sender and receiver differs in currency type");
            }

            // Balance transaction
            senderAccount.Balance -= transferAmount;
            receiverAccount.Balance += transferAmount;

            // Creates an audit table for transactions
            var balanceTransactionRecord = new BalanceTransaction()
            {
                SenderBankAccountId = senderAccountId,
                ReceiverBankAccountId = receiverAccountId,
                Amount = transferAmount,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.BalanceTransactions.AddAsync(balanceTransactionRecord);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        return TransferBetweenAccountsResult.Success("Transaction completed successfully");
    }

    private TransferBetweenAccountsResult MapToTransferResponse(Error<AccountBalanceTransactionContextError> error)
    {
        return error.Code switch
        {
            AccountBalanceTransactionContextError.NegativeOrZeroAmount =>
                TransferBetweenAccountsResult.Fail(TransferBalanceBetweenAccountError.NegativeOrZeroAmount, error.Message),

            AccountBalanceTransactionContextError.NotEnoughBalance =>
                TransferBetweenAccountsResult.Fail(TransferBalanceBetweenAccountError.NotEnoughBalance, error.Message),

            AccountBalanceTransactionContextError.ReceiverBankAccountNotFound => 
                TransferBetweenAccountsResult.Fail(TransferBalanceBetweenAccountError.ReceiverBankccountNotFound, error.Message),

            AccountBalanceTransactionContextError.SameBankAccounts =>
                TransferBetweenAccountsResult.Fail(TransferBalanceBetweenAccountError.SameBankAccount, error.Message),

            AccountBalanceTransactionContextError.SenderBankAccountNotFound =>
                TransferBetweenAccountsResult.Fail(TransferBalanceBetweenAccountError.SenderBankAccountNotFound, error.Message),

            _ => throw new InvalidOperationException("Unmapped exception")
            
        };
    }

    public async Task<GetAllTransactionsResult> GetAllTransactionAsync()
    {
        var responseDto = await _dbContext.BalanceTransactions.Select(e => new BalanceTransactionGetResponse()
        {
            BalanceTransactionId = e.BalanceTransactionId,
            ReceiverBankAccountId = e.ReceiverBankAccountId,
            SenderBankAccountId = e.SenderBankAccountId,
            Amount = e.Amount
        }).ToListAsync();

        return GetAllTransactionsResult.Success(responseDto);
    }

}

