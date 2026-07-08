namespace CoreBankingWebApplication.Models.Entities;

/// <summary>
/// This entity represents a reserve of an specific currency, f.e 1,000,000 Dollars for exchange purposes
/// </summary>

public class CurrencyExchangeReserve
{
    public int CurrencyExchangeReserveId { get; set; }
    public int CurrencyId { get; set; }
    public Currency Currency { get; set; } = null!;
    public decimal Balance { get; set; }

    public ICollection<CurrencyExchange>? DebitedExchangeCurrencies { get; set; }
    public ICollection<CurrencyExchange>? AccreditedExchangeCurrencies { get; set; }

}
