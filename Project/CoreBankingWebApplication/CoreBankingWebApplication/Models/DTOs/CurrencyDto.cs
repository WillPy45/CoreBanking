using CoreBankingWebApplication.Models.Entities;


namespace CoreBankingWebApplication.Models.DTOs;

public class CurrencyDto
{
    public required string Code { get; set; }


}

public static class CurrencyDtoExtensions
{
    public static Currency ToEntity(this CurrencyDto dto)
    {
        return new Currency()
        {
            Code = dto.Code
        };
    }
}
