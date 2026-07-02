using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services
{
    public class CurrencyExchangeReserveCrudService
    {
        private readonly ApplicationDbContext _dbContext;
        public CurrencyExchangeReserveCrudService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<string>> CreateCurrencyReserve()       
    }
}