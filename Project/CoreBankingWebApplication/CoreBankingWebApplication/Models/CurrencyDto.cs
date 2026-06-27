namespace CoreBankingWebApplication.Models
{
    public class CurrencyDto
    {
        public string Code {get; set; }


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
}