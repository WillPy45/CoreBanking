namespace CoreBankingWebApplication.Models.DTOs;


public class CurrencyExchangeRequestDto
{

    // Changes rates is automatically managed by others services,
    // so is unnecesary to specific the currency
    // I want to sell some USD from my bank account (SenderAccount) ...
    public int SenderBankAccountId { get; set; }

    // ... and receive in this account

    public int ReceiverBankAccountId { get; set; }

    public decimal Amount { get; set; }

}
