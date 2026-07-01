namespace CoreBankingWebApplication.Models
{
    // This entity represents a reserve of an specific currency, f.e 1,000,000 Dollars for exchange purposes

    public class CurrencyExchangeReserve
    {
        public int CurrencyExchangeReserveId { get; set; }
        public int CurrencyId { get; set; }
        public Currency? Currency { get; set; }
        public decimal Balance { get; set; }

        public ICollection<CurrencyExchange>? DebitedExchangeCurrencies {get; set;}
        public ICollection<CurrencyExchange>? AccreditedExchangeCurrencies {get; set;}

    }
}