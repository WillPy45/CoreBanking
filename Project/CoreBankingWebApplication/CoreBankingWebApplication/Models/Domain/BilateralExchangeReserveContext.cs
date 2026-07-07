using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CoreBankingWebApplication.Models.Domain;

using CreateValidContextResult = Result<BilateralExchangeReserveContext, BilateraExchangeReserveContextError>;

public record BilateralExchangeReserveContext(CurrencyExchangeReserve AccreditedReserve, CurrencyExchangeReserve DebitedReserve, decimal DebitedAmount)
{
    public static CreateValidContextResult CreateValidContext(CurrencyExchangeReserve? accreditedReserve, CurrencyExchangeReserve? debitedReserve, decimal debitedAmount)
    {
        if (debitedAmount < 0.0m)
        {
            return CreateValidContextResult.Fail(BilateraExchangeReserveContextError.NegativeOrZeroAmount, "Amount must be positive number");
        }
      
        // Ensures that both reserves exists
        if (accreditedReserve is null)
        {
            return CreateValidContextResult.Fail(BilateraExchangeReserveContextError.AccreditedReserveDoesntExist,
                $"Accredited reserve doesnt exists");
        }

        if (debitedReserve is null)
        {
            return CreateValidContextResult.Fail(BilateraExchangeReserveContextError.DebitedReserveDoesntExist,
                $"Debited reserve doesnt exists");
        }

        // Ensures that debited reserve has enough balance
        if (debitedReserve.Balance < debitedAmount)
        {
            return CreateValidContextResult.Fail(BilateraExchangeReserveContextError.HasNotEnoughBalanceInCurrencyReserve, "Bank reserve has not enough balance to deposit in receiver account");
        }

        return CreateValidContextResult.Success(new BilateralExchangeReserveContext(accreditedReserve, debitedReserve, debitedAmount));
    }


}

public enum BilateraExchangeReserveContextError
{
    NegativeOrZeroAmount,
    AccreditedReserveDoesntExist,
    DebitedReserveDoesntExist,
    HasNotEnoughBalanceInCurrencyReserve


}
