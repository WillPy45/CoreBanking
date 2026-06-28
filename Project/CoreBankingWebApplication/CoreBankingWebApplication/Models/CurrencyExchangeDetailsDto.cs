namespace CoreBankingWebApplication.Models
{

    public class CurrencyExchangeResponseDto
    {

        // Changes rates is automatically managed by others services,
        // so is unnecesary to specific the currency
        // I want to sell some USD from my bank account (SenderAccount) ...
        public int SenderBankAccountId { get; set; } 

        // ... and receive in this account

        public int ReceiverBankAccountId {get; set; } 

        public decimal GrossAmount { get; set; }
        public decimal BankRateInPercentage {get; set;}
        public decimal BankRateAmount {get; set;}
        public decimal NetAmount {get; set; }

    }
}