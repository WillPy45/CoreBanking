namespace CoreBankingWebApplication.Models
{
    // Data transfer object for represents the final result of an exchange operation

    public class CurrencyExchangeDetailsDto
    {
        public int CurrencyExchangeId { get; set; }


        // Changes rates is automatically managed by others services,
        // so is unnecesary to specific the currency
        // I want to sell some USD from my bank account (SenderAccount) ...
        public int SenderBankAccountId { get; set; } // The currency type is here
        
        
        // for buy from this reserve ...
        public int CurrencyExchangeReserveId {get; set; } // The currency type is here
        
        // ... and receive in this account

        public int ReceiverBankAccountId {get; set; } 

        public decimal GrossAmount { get; set; }
        public decimal BankRateInPercentage {get; set;}
        public decimal BankRateAmount {get; set;}
        public decimal NetAmount {get; set; }

    }
}