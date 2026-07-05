namespace CoreBankingWebApplication.Models.Entities;

public class Currency
{
    public int CurrencyId { get; set; }
    public required string Code { get; set; }

    public ICollection<BankAccount>? BankAccounts { get; set; }
    public CurrencyExchangeReserve? ExchangeReserve { get; set; }
}
