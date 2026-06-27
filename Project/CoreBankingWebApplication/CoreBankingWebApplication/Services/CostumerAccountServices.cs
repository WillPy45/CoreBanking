using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services
{
    public class CostumerAccountServices
    {
        private readonly ApplicationDbContext _dbContext;

        public CostumerAccountServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        } 

        public async Task CreateAsync(CustomerDTO dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                
                Customer entity = dto.ToEntity();
                
                await _dbContext.Customers.AddAsync(entity);

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
        
            } 

            catch (Exception exception)
            {
                await transaction.RollbackAsync();
                throw;
            }            
            
        }

    }
}