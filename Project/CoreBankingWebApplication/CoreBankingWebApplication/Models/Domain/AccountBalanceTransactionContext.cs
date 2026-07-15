using CoreBankingWebApplication.Common;
using CoreBankingWebApplication.Models.Entities;

namespace CoreBankingWebApplication.Models.Domain;


using CreatePairResult = Result<AccountBalanceTransactionContext, AccountBalanceTransactionContextError>;


/// <summary>
/// Represents a bilateral transaction context between two bank accounts.
/// </summary>
/// <param name="Sender">The bank account that sends the balance</param>
/// <param name="Receiver">The bank account that receives the balance</param>
/// <param name="DebitedAmount">The amount to be debited from the sender</param>
/// <remarks>
/// Note: The <paramref name="DebitedAmount"/> refers only to the amount 
/// debited from the sender account. The receiver may obtain a diferent amount calculated by
/// other services, helpers, etc
/// </remarks>
public record AccountBalanceTransactionContext(BankAccount Sender, BankAccount Receiver, decimal DebitedAmount)
{

    /// <summary>
    /// Creates a valid bilateral transaction context, if all intrinsic rules are satisfied
    /// </summary>
    /// <param name="sender">The bank account that sends the balance</param>
    /// <param name="receiver">The bank account that receives the balance</param>
    /// <param name="debitedAmount">The amount to be debited from the sender</param>
    /// <remarks>
    /// To create a valid context, these rules must be followed:
    /// - Ensure accounts exist
    /// - Accounts are distinct
    /// - Sender account has sufficient balance
    /// - The amount is positive.
    /// </remarks>
    /// <returns>A result object indicating success with a valid <see cref="AccountBalanceTransactionContext"/>
    /// or failure with <see cref="AccountBalanceTransactionContextError"/> that indicates the reason</returns>
    public static Result<AccountBalanceTransactionContext, AccountBalanceTransactionContextError> CreateValidContext(BankAccount? sender, BankAccount? receiver, decimal debitedAmount)
    {
        if (sender is null)
        {
            return CreatePairResult.Fail(AccountBalanceTransactionContextError.SenderBankAccountNotFound, "Sender bank account not found");
        }
        if (receiver is null)
        {
            return CreatePairResult.Fail(AccountBalanceTransactionContextError.ReceiverBankAccountNotFound, "Receiver bank account not found");
        }
        if (sender.BankAccountId == receiver.BankAccountId)
        {
            return CreatePairResult.Fail(AccountBalanceTransactionContextError.SameBankAccounts, "A bilateral transaction requires two different bank accounts");
        }
        if (debitedAmount <= 0.0m)
        {
            return CreatePairResult.Fail(AccountBalanceTransactionContextError.NegativeOrZeroAmount, "Amount must be positive");
        }
        if (sender.Balance < debitedAmount)
        {
            return CreatePairResult.Fail(AccountBalanceTransactionContextError.NotEnoughBalance, "Not enough balance to transfer");
        }

        return CreatePairResult.Success(new AccountBalanceTransactionContext(sender, receiver, debitedAmount));
    }

}

/// <summary>
/// Enum that represents possibles invalid states for the transaction between accounts
/// </summary>
public enum AccountBalanceTransactionContextError
{
    ReceiverBankAccountNotFound,
    SenderBankAccountNotFound,
    SameBankAccounts,
    NegativeOrZeroAmount,
    NotEnoughBalance

}
