using CoreBankingWebApplication.Common;
using CoreBankingWebApplication.Models.Entities;

namespace CoreBankingWebApplication.Models.Domain
{    

    using CreatePairResult = Result<AccountBilateralTransactionContext, AccountBilateralTransactionContextError>;
    // This class is used when exists a transactionr relationship between two accounts and needs both for the operation
    public record AccountBilateralTransactionContext(BankAccount Sender, BankAccount Receiver, decimal DebitedAmount)
    {

        //  Intrisic pair transactions rules must be applied here
        public static Result<AccountBilateralTransactionContext, AccountBilateralTransactionContextError> CreateValidContext(BankAccount? sender, BankAccount? receiver, decimal debitedAmount)
        {
            if (sender is null) 
            { 
                return CreatePairResult.Fail(AccountBilateralTransactionContextError.SenderBankAccountNotFound, "Sender bank account not found");
            }
            if (receiver is null)
            {
                return CreatePairResult.Fail(AccountBilateralTransactionContextError.ReceiverBankAccountNotFound, "Receiver bank account not found");
            }
            if (sender.BankAccountId == receiver.BankAccountId)
            {
                return CreatePairResult.Fail(AccountBilateralTransactionContextError.SameBankAccounts, "A bilateral transaction requires two different bank accounts");
            }
            if (debitedAmount <= 0.0m)
            {
                return CreatePairResult.Fail(AccountBilateralTransactionContextError.NegativeOrZeroAmount, "Amount must be positive");
            }
            if (sender.Balance < debitedAmount)
            {
                return CreatePairResult.Fail(AccountBilateralTransactionContextError.NotEnoughBalance, "Not enough balance to transfer");
            }

            return CreatePairResult.Success(new AccountBilateralTransactionContext(sender, receiver, debitedAmount));
        }
    }

    public enum AccountBilateralTransactionContextError
    {
        ReceiverBankAccountNotFound,
        SenderBankAccountNotFound,
        SameBankAccounts,
        NegativeOrZeroAmount,
        NotEnoughBalance
        
    }
}