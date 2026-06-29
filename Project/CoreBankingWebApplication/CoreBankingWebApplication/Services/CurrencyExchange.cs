using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;
using CoreBankingWebApplication.Migrations;

namespace CoreBankingWebApplication.Services
{
    public class CurrencyExchangeService
    {
        private readonly ApplicationDbContext _dbContext;

        public CurrencyExchangeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<string>> Exchange(CurrencyExchangeRequestDto requestDto)
        {
            
            throw new NotImplementedException("Not implemented endpoint");
        }
       
        
    }
}