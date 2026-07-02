using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services
{
    public interface ICurrencyRateService
    {
        Task<Result<decimal>> GetConversionRateAsync(string fromCurrencyCode, string toCurrencyCode);
        
    }

}