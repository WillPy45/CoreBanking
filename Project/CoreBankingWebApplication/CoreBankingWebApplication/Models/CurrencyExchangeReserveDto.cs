namespace CoreBankingWebApplication.Models
{

    public class CurrencyExchangeReserveCreateRequestDto
    {
        public int CurrencyId { get; set; }
        public decimal Balance { get; set; }

    }
}