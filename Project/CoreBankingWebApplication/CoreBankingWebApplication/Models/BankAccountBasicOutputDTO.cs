namespace CoreBankingWebApplication.Models
{
   public class BankAccountBasicOutputDTO
    {
        public int BankAccountId { get; set; }
        public int CustomerId { get; set; } //references to a CustomerID
        public decimal Balance { get; set; } //At the moment we just manage PY currency
        
        public BankAccountBasicOutputDTO(){}

        public static BankAccountBasicOutputDTO? FromEntity(BankAccount? entity)
        {
            if (entity is null) return null;

            return new BankAccountBasicOutputDTO()
            {
                BankAccountId = entity.BankAccountId,
                CustomerId = entity.CustomerId,
                Balance = entity.Balance
            };
        }
    }
}