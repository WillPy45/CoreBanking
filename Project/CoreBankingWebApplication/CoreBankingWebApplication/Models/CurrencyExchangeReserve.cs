namespace CoreBankingWebApplication.Models
{

    public class CurrencyExchangeReserve
    {
        public int CurrencyExchangeReserveId { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public decimal Balance { get; set; }

    }
}