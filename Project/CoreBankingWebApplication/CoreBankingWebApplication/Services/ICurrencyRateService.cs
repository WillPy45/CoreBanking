using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services
{
    public interface ICurrencyRateService
    {
        Task<Result<decimal>> GetConversionRateAsync(string fromCurrencyCode, string toCurrencyCode);
        
    }

}