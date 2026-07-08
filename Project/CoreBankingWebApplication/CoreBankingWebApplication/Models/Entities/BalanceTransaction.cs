namespace CoreBankingWebApplication.Models.Entities;


/// <summary>
/// This entity represents an balance transaction audit table
/// </summary>
public class BalanceTransaction
{
    public int BalanceTransactionId { get; set; }

    public int SenderBankAccountId { get; set; } // Account that has been debited
    public BankAccount SenderBankAccount { get; set; } = null!;   // Navigation property

    public int ReceiverBankAccountId { get; set; } // Account that has been acredited
    public BankAccount ReceiverBankAccount { get; set; } = null!; // Navigation property

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }
}

