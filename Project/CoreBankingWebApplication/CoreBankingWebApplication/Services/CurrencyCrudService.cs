using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Common;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services
{
    public class CurrencyCrudService
    {
        private readonly ApplicationDbContext _dbContext;

        public CurrencyCrudService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<string>> CreateAsync(CurrencyDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();


            try
            {
                await _dbContext.Currencies.AddAsync(dto.ToEntity());
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

            }catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                return Result<string>.Fail(new Error("Currency.AlreadyExists", "Already existing currency"));
            }catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return Result<string>.Success("Currency created succesfully");
        }

        public async Task<Result<ICollection<CurrencyBasicOutputDto>>> GetCurrencies()
        {
           return Result<ICollection<CurrencyBasicOutputDto>>.Success( 
                await _dbContext.Currencies.Select((e) => CurrencyBasicOutputDto.FromEntity(e))
                .ToListAsync()
            );
        }
    }

}