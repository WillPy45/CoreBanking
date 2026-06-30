using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {

        public async Task<Result<decimal>> GetConversionRateAsync(string fromCurrencyCode, string toCurrencyCode)
        {
            if (fromCurrencyCode == "PYG" && toCurrencyCode == "USD") 
                return Result<decimal>.Success(1m/6100m);

            if (fromCurrencyCode == "USD" && toCurrencyCode == "PYG")
                return Result<decimal>.Success(5900/1m);

            return Result<decimal>.Fail(
                new Error("CurrencyRate.ConversionFailed",
                    $"Undefined conversion for from {fromCurrencyCode} to {toCurrencyCode}"));
        }
    }
}