namespace CoreBankingWebApplication.Models.Entities
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }

        public int CustomerId { get; set; } //references to a CustomerID
        public Customer? Customer { get; set; } // Navigation property

        
        public int CurrencyId { get; set; }
        public Currency? Currency { get; set; }
        
        public decimal Balance { get; set; } 

        public DateTime CreateAt {get; set;}


        
        /* reverse navigation property for transaction that this BankAccount is involved
        BECAUSE is referred in RECEIVE and SENDER role, a BankAccount may was a sender or receiver, or both
        
        */
        public ICollection<BalanceTransaction>? SentTransactions { get; set; }
        public ICollection<BalanceTransaction>? ReceivedTransactions { get; set; }

        // Exchange operations (rnp)
        public ICollection<CurrencyExchange>? SentCurrencyExchanges { get; set; }
        public ICollection<CurrencyExchange>? ReceivedCurrencyExchanges { get; set; }

    }

    
}
