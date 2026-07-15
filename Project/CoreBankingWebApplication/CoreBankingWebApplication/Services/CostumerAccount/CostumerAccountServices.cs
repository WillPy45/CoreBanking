using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CoreBankingWebApplication.Services;

public class CostumerAccountServices
{

    private readonly ApplicationDbContext _dbContext;

    public CostumerAccountServices(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(CustomerDTO dto)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {

            Customer entity = dto.ToEntity();

            await _dbContext.Customers.AddAsync(entity);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

        }

        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

    }

}
