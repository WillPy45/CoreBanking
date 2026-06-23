namespace CoreBankingWebApplication.Models
{

    //Its just a bank, so, BankId is not necessary because operations are internals
    public class BankAccount
    {
        public int BankAccountId { get; set; }
        public int CustomerId { get; set; } //references to a CustomerID
        public Customer Customer { get; set; } // Navigation property

        public decimal Balance { get; set; } //At the moment we just manage PY currency
    }
}
