namespace CoreBankingWebApplication.Models
{
    public class BankAccountInputDTO
    {
        public int CustomerId { get; set; } //references to a CustomerID
        public decimal Balance { get; set; } //At the moment we just manage PY currency

        public BankAccountInputDTO(BankAccount entity)
        {
            this.CustomerId = entity.CustomerId;
            this.Balance = entity.Balance;
        }

        public BankAccountInputDTO(){}
    }
    
      public static class BankAccountInputDTOExtensions
    {
        // Cambiado a 'ToEntity' o 'MapTo' para que sea más claro lo que hace
        public static BankAccount ToEntity(this BankAccountInputDTO dto)
        {
            return new BankAccount
            {
              CustomerId = dto.CustomerId,
              Balance = dto.Balance  
            };
        }

        public static void UpdateEntity(this BankAccountInputDTO dto, BankAccount entity)
        {
            entity.Balance = dto.Balance;
            entity.CustomerId = dto.CustomerId;
        }
    }
}