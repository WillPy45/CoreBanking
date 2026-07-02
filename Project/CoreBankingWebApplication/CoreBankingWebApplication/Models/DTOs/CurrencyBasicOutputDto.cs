using CoreBankingWebApplication.Models.Entities;


namespace CoreBankingWebApplication.Models.DTOs
{
    public class CurrencyBasicOutputDto
    {
        public int CurrencyId {get; set; }
        public required string Code {get; set; }

        public static CurrencyBasicOutputDto FromEntity(Currency entity)
        {
            return new CurrencyBasicOutputDto()
            {
                CurrencyId = entity.CurrencyId,
                Code = entity.Code
            };
        }
    }
}