namespace CoreBankingWebApplication.Models.Entities;

/// <summary>
/// This entity represents an audit table for Currency Exchange operations
/// </summary>

public class CurrencyExchange
{
    public int CurrencyExchangeId { get; set; }

    public int SenderBankAccountId { get; set; }
    public BankAccount SenderBankAccount { get; set; } = null!; // nav. prop.

    // for buy from this reserve ...
    public int DebitedExchangeReserveId { get; set; } // The currency type is here
    public CurrencyExchangeReserve DebitedExchangeReserve { get; set; } = null!;

    public int AccreditedExchangeReserveId { get; set; }
    public CurrencyExchangeReserve AccreditedExchangeReserve { get; set; } = null!;


    // ... and receive in this account

    public int ReceiverBankAccountId { get; set; }
    public BankAccount ReceiverBankAccount { get; set; } = null!; // nav. prop.

    // Original amount (before exchange)
    public decimal SourceAmount { get; set; }

    // Price of each bought unit, f.e 1 USD = 6500 PYG
    public decimal BankUnitPrice { get; set; }
    // Amount received as bought currency
    public decimal GrossAmount { get; set; }

    // Spread for using the service
    public decimal BankDiscountInPercentage { get; set; }

    public decimal BankDiscount { get; set; }

    public decimal NetAmount { get; set; }

    public DateTime CreateAt { get; set; }

}
