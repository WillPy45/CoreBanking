using CoreBankingWebApplication.Models.Entities;

namespace CoreBankingWebApplication.Models.DTOs
{
   public class BankAccountBasicOutputDTO
    {
        public int BankAccountId { get; set; }
        public int CustomerId { get; set; } //references to a CustomerID

        public int CurrencyId { get; set; }

        public decimal Balance { get; set; } 
        
        public BankAccountBasicOutputDTO(){}

        public static BankAccountBasicOutputDTO FromEntity(BankAccount entity)
        {
            return new BankAccountBasicOutputDTO()
            {
                BankAccountId = entity.BankAccountId,
                CustomerId = entity.CustomerId,
                CurrencyId = entity.CurrencyId,
                Balance = entity.Balance
            };
        }
    }
}