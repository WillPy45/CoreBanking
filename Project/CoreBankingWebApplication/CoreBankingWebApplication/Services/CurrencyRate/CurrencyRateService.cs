using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services;

using CurrencyRateResult = Result<decimal, CurrencyRateServiceError>;
public class CurrencyRateService : ICurrencyRateService
{

    public async Task<CurrencyRateResult> GetConversionRateAsync(string fromCurrencyCode, string toCurrencyCode)
    {
        if (fromCurrencyCode == "PYG" && toCurrencyCode == "USD")
            return CurrencyRateResult.Success(1m / 6100m);

        if (fromCurrencyCode == "USD" && toCurrencyCode == "PYG")
            return CurrencyRateResult.Success(5900 / 1m);

        return CurrencyRateResult.Fail(
            CurrencyRateServiceError.ConversionFailed,
            $"Undefined conversion for from {fromCurrencyCode} to {toCurrencyCode}");
    }
}
