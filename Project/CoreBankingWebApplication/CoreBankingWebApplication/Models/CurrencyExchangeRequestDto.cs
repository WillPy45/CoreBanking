namespace CoreBankingWebApplication.Models
{

    // Is just a Data Transfer Object to define some parameters for
    public class CurrencyExchangeRequestDto
    {

        // Changes rates is automatically managed by others services,
        // so is unnecesary to specific the currency
        // I want to sell some USD from my bank account (SenderAccount) ...
        public int SenderBankAccount { get; set; } 

        // ... and receive in this account

        public int ReceiverBankAccount {get; set; } 

        public decimal Amount { get; set; }

    }
}