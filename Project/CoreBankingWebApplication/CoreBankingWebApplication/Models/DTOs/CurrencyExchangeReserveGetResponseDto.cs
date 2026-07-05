using CoreBankingWebApplication.Models.Entities;

namespace CoreBankingWebApplication.Models.DTOs;

// This entity represents a reserve of an specific currency, f.e 1,000,000 Dollars for exchange purposes

public class CurrencyExchangeReserveGetResponseDto
{
    public int CurrencyExchangeReserveId { get; set; }
    public int CurrencyId { get; set; }
    public decimal Balance { get; set; }

}
