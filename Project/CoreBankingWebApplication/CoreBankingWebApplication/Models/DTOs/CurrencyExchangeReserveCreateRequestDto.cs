using CoreBankingWebApplication.Models.Entities;


namespace CoreBankingWebApplication.Models.DTOs;


public class CurrencyExchangeReserveCreateRequestDto
{
    public int CurrencyId { get; set; }
    public decimal Balance { get; set; }

}
