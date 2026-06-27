namespace CoreBankingWebApplication.Models
{
    public class CurrencyBasicOutputDto
    {
        public int CurrencyId {get; set; }
        public string Code {get; set; }

        public static CurrencyBasicOutputDto? FromEntity(Currency entity)
        {
            return new CurrencyBasicOutputDto()
            {
                CurrencyId = entity.CurrencyId,
                Code = entity.Code
            };
        }
    }
}