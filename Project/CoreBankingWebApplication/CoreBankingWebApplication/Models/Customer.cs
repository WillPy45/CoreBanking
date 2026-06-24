namespace CoreBankingWebApplication.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Address { get; set; }

        //Bank accounts (A customer can have 0, 1, N bank accounts, a bank account is just for a customer
        public ICollection<BankAccount> BankAccounts { get; set; } // Reverse navigation property

        //Navigation property for transaction that this costumer is involved
        public ICollection<BalanceTransaction> BalanceTransactions { get; set; }

    }
}
