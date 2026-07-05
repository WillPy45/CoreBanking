using CoreBankingWebApplication.Models.Entities;

namespace CoreBankingWebApplication.Models.DTOs;

public class BankAccountInputDTO
{
    public int CustomerId { get; set; } //references to a CustomerID

    public int CurrencyId { get; set; } //Fk to determinate currency
    public decimal Balance { get; set; }

    public BankAccountInputDTO(BankAccount entity)
    {
        this.CustomerId = entity.CustomerId;
        this.CurrencyId = entity.CurrencyId;
        this.Balance = entity.Balance;

    }

    public BankAccountInputDTO() { }
}

public static class BankAccountInputDTOExtensions
{
    public static BankAccount ToEntity(this BankAccountInputDTO dto)
    {
        return new BankAccount
        {
            CustomerId = dto.CustomerId,
            Balance = dto.Balance,
            CurrencyId = dto.CurrencyId
        };
    }

    public static void UpdateEntity(this BankAccountInputDTO dto, BankAccount entity)
    {
        entity.Balance = dto.Balance;
        entity.CustomerId = dto.CustomerId;
        entity.CurrencyId = dto.CurrencyId;
    }
}
