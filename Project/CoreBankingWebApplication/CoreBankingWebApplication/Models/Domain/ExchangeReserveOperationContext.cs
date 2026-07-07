using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Common;

namespace CoreBankingWebApplication.Models.Domain;

using CreateValidContextResult = Result<ExchangeReserveOperationContext, ExchangeReserveOperationContextError>;

/// <summary>
/// Represents a implication between two reserves in an exchange context.
/// Applies intrinsic validation rules to ensure reserves exist, are distinct, currency type are differents,
/// have sufficient balance for sends, and the DebitedAmount is positive.
/// Note: The <paramref name="DebitedAmount"/> refers only to the amount 
/// debited from the DebitedReserve, the destination of that DebitedAmount is managed by others systems.
/// 
/// </summary>
public record ExchangeReserveOperationContext(CurrencyExchangeReserve AccreditedReserve, CurrencyExchangeReserve DebitedReserve, decimal DebitedAmount)
{

    /// <summary>
    /// Creates an valid implication context between two bank reserves, if all intrinsic rules are satisfied
    /// </summary>
    /// <param name="accreditedReserve">The bank reserve that receives an increase of balance from any source products of the exchange</param>
    /// <param name="debitedReserve">The bank reserve that balance is debited"/></param>
    /// <param name="debitedAmount">The amount to be debited from the debited reserve</param>
    /// <returns>A result object indicating success with a valid <see cref="ExchangeReserveOperationContext"/>
    /// or failure with <see cref="ExchangeReserveOperationContextError"/> that indicates the reason</returns>
    public static CreateValidContextResult CreateValidContext(CurrencyExchangeReserve? accreditedReserve, CurrencyExchangeReserve? debitedReserve, decimal debitedAmount)
    {
        // Ensures that debited amount is positive
        if (debitedAmount <= 0.0m)
        {
            return CreateValidContextResult.Fail(ExchangeReserveOperationContextError.NegativeOrZeroAmount, "Amount must be positive number");
        }
      
        // Ensures that both reserves exists
        if (accreditedReserve is null)
        {
            return CreateValidContextResult.Fail(ExchangeReserveOperationContextError.AccreditedReserveDoesntExist,
                $"Accredited reserve doesnt exists");
        }

        if (debitedReserve is null)
        {
            return CreateValidContextResult.Fail(ExchangeReserveOperationContextError.DebitedReserveDoesntExist,
                $"Debited reserve doesnt exists");
        }

        // Ensures that are not the same accounts
        if (debitedReserve.CurrencyExchangeReserveId == accreditedReserve.CurrencyExchangeReserveId)
        {
            return CreateValidContextResult.Fail(ExchangeReserveOperationContextError.SameReserves, "reserves must be differents");
        }

        // Ensures that uses diferent currencies
        if (accreditedReserve.Currency.CurrencyId == debitedReserve.Currency.CurrencyId)
        {
            return CreateValidContextResult.Fail(ExchangeReserveOperationContextError.SameCurrencyType, "Currencies must be different");
        }

        // Ensures that debited reserve has enough balance
        if (debitedReserve.Balance < debitedAmount)
        {
            return CreateValidContextResult.Fail(ExchangeReserveOperationContextError.HasNotEnoughBalanceInCurrencyReserve, "Bank reserve has not enough balance to deposit in receiver account");
        }

        return CreateValidContextResult.Success(new ExchangeReserveOperationContext(accreditedReserve, debitedReserve, debitedAmount));
    }


}


/// <summary>
/// Enum that represents possibles invalid states for the context
/// </summary>
public enum ExchangeReserveOperationContextError
{
    NegativeOrZeroAmount,
    AccreditedReserveDoesntExist,
    DebitedReserveDoesntExist,
    HasNotEnoughBalanceInCurrencyReserve,
    SameReserves,
    SameCurrencyType

}
