using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Models.ExtensionMethods
{
    public static class BankAccountExtensionMethods
    {
        public static async Task<BankAccount?> GetBankAccountWithCurrency(this IQueryable<BankAccount> bankAccounts, int id)
        {
            return await bankAccounts
                .Include(e => e.Currency)
                .FirstOrDefaultAsync(e => e.BankAccountId == id);
        }

    }


}