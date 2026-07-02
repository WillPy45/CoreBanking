using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {

        public async Task<Result<decimal, CurrencyRateServiceError>> GetConversionRateAsync(string fromCurrencyCode, string toCurrencyCode)
        {
            if (fromCurrencyCode == "PYG" && toCurrencyCode == "USD") 
                return Result<decimal, CurrencyRateServiceError>.Success(1m/6100m);

            if (fromCurrencyCode == "USD" && toCurrencyCode == "PYG")
                return Result<decimal, CurrencyRateServiceError>.Success(5900/1m);

            return Result<decimal, CurrencyRateServiceError>.Fail(
                CurrencyRateServiceError.ConversionFailed,
                $"Undefined conversion for from {fromCurrencyCode} to {toCurrencyCode}");
        }
    }
}