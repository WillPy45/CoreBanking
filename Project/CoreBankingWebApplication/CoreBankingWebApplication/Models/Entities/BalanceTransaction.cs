namespace CoreBankingWebApplication.Models.Entities
{

    /*
        This model represents a balance transaction between two BankAccount,
        this describe ammount transaction, who is debited and who is acredited,
        with a DateTime to remember the moment os the operation. 

        A balance transaction record represents just for an pair of BankAccountA (sender) and BankAccountB (receiver)
    */
    public class BalanceTransaction
    {
       public int BalanceTransactionId { get; set; } 

       public int SenderBankAccountId { get; set; } // Account that has been debited
       public BankAccount? SenderBankAccount { get; set; } // Navigation property

       public int ReceiverBankAccountId { get; set; } // Account that has been acredited
       public BankAccount? ReceiverBankAccount { get; set; } // Navigation property
       
       public decimal Amount {get; set; }

       public DateTime CreatedAt { get; set; }
    }

}