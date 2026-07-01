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
            public BankAccount? SenderBankAccount { get; set; } // nav. prop.
            
            // for buy from this reserve ...
            public int DebitedExchangeReserveId {get; set; } // The currency type is here
            public CurrencyExchangeReserve? DebitedExchangeReserve { get; set; }

            public int AccreditedReserveId { get; set; }
            public CurrencyExchangeReserve? AccreditedExchangeReserve { get; set; }


            // ... and receive in this account

            public int ReceiverBankAccountId { get; set; } 
            public BankAccount? ReceiverBankAccount { get; set; } // nav. prop.

            // Original amount (before exchange)
            public decimal SourceAmount { get; set; }

            // Price of each bought unit, f.e 1 USD = 6500 PYG
            public decimal BankUnitPrice { get; set; }
            // Amount received as bought currency
            public decimal GrossAmount { get; set; }

            // Spread for using the service
            public decimal BankDiscountInPercentage {get; set;}
            // Bank discount
            public decimal BankDiscount {get; set;}

            // Net amount
            public decimal NetAmount {get; set; }

            public DateTime CreateAt { get; set; }

        }
    }