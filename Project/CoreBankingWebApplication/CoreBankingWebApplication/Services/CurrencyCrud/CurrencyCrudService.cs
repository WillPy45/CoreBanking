using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services;

//Aliases
using CurrencyCreateResult = Result<string, CurrencyCrudCreateServiceError>;
using CurrencyGetAllResult = Result<ICollection<CurrencyBasicOutputDto>, CurrencyCrudGetAllServiceError>;

public class CurrencyCrudService
{
    private readonly ApplicationDbContext _dbContext;

    public CurrencyCrudService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CurrencyCreateResult> CreateAsync(CurrencyDto dto)
    {

        using var transaction = await _dbContext.Database.BeginTransactionAsync();


        try
        {
            await _dbContext.Currencies.AddAsync(dto.ToEntity());
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            return CurrencyCreateResult.Fail(CurrencyCrudCreateServiceError.AlreadyExists, "Already existing currency");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        return CurrencyCreateResult.Success("Currency created succesfully");
    }

    public async Task<CurrencyGetAllResult> GetCurrencies()
    {
        return CurrencyGetAllResult.Success(
             await _dbContext.Currencies.Select((e) => CurrencyBasicOutputDto.FromEntity(e))
             .ToListAsync()
         );
    }
}

