using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services
{
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
        /// 3. Verify sufficient funds in the sender account.
        /// 4. Update balances and write audit log.
        /// </remarks>
        public async Task<Result<string>> TransferAsync(BalanceTransactionDTO transferDto)
        {

            int senderAccountId = transferDto.SenderBankAccountId;
            int receiverAccountId = transferDto.ReceiverBankAccountId;
            decimal transferAmount = transferDto.Amount;

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Prevent self-transfers
                if (senderAccountId == receiverAccountId)
                {
                    return Result<string>.Fail(
                        new Error("Transfer.TransferItSelf", "Accounts cannot be transferred to yourself"));
                }    

                // Validate that ammount balance is positive
                if (transferAmount < 0.0m)
                {
                    return Result<string>.Fail(
                        new Error("Transfer.NegativeAmount", "Ammount must be positive number"));
                }

                // Fetch both accounts tracked by EF in a single roundtrip to optimize database performance
                var recordsRetrieved = await _dbContext.BankAccounts
                    .Where(e => e.BankAccountId == senderAccountId || e.BankAccountId == receiverAccountId)
                    .ToListAsync();
                
                var senderAccount = recordsRetrieved.Find(account => account.BankAccountId == senderAccountId);
                var receiverAccount = recordsRetrieved.Find(account => account.BankAccountId == receiverAccountId);

                //Ensures that accounts exists
                if (senderAccount is null)
                {
                    return Result<string>.Fail(
                        new Error("Account.SenderAccountNotFound", "Sender account not found"));
                }

                if (receiverAccount is null)
                {
                    return Result<string>.Fail(
                        new Error("Account.ReceiverAccountNotFound", "Receiver account not found"));
                }

                // Ensures that sender has enough balance
                if (senderAccount.Balance < transferAmount)
                {
                    return Result<string>.Fail(
                        new Error("Transfer.NotEnoughBalance", "Sender balance is not enough for this transaction"));
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

            return Result<string>.Success("Transaction completed successfully");
        }

    }

}