namespace CoreBankingWebApplication.Models
{
    // Entity
    // This entity represents an audit table for Currency Exchange operations

    public class CurrencyExchange
    {
        public int CurrencyExchangeId { get; set; }


        // Changes rates is automatically managed by others services,
        // so is unnecesary to specific the currency
        // I want to sell some USD from my bank account (SenderAccount) ...
        public int SenderBankAccountId { get; set; } 
        public BankAccount SenderBankAccount { get; set; } // nav. prop.
        
        // for buy from this reserve ...
        public int CurrencyExchangeReserveId {get; set; } // The currency type is here
        public CurrencyExchangeReserve CurrencyExchangeReserve { get; set; }

        // ... and receive in this account

        public int ReceiverBankAccountId { get; set; } 
        public BankAccount ReceiverBankAccount { get; set; } // nav. prop.

        // Original amount (before exchange)
        public decimal SourceAmount { get; set; }

        // Price of each bought unit, f.e 1 USD = 6500 PYG
        public decimal UnitPrice { get; set; }
        // Amount received as bought currency
        public decimal GrossAmount { get; set; }

        // Spread for using the service
        public decimal BankRateInPercentage {get; set;}
        // Bank discount
        public decimal BankRateAmount {get; set;}

        // Net amount
        public decimal NetAmount {get; set; }

        public DateTime CreateAt { get; set; }

    }
}