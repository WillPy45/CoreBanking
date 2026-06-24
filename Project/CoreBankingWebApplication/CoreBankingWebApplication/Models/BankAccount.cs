namespace CoreBankingWebApplication.Models
{

    //Its just a bank, so, BankId is not necessary because operations are internals
    public class BankAccount
    {
        public int BankAccountId { get; set; }

        public int CustomerId { get; set; } //references to a CustomerID
        public Customer Customer { get; set; } // Navigation property

        public decimal Balance { get; set; } //At the moment we just manage PY currency
        
        /* reverse navigation property for transaction that this BankAccount is involved
        BECAUSE is referred in RECEIVE and SENDER role, a BankAccount may was a sender or receiver, or both
        
        */
        public ICollection<BalanceTransaction> SentTransactions { get; set; }
        public ICollection<BalanceTransaction> ReceivedTransactions { get; set; }

    }

    
}
