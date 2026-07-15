using CoreBankingWebApplication.Common;
using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Services;

using CurrencyExchangeReserveGetAllResult = Result<ICollection<CurrencyExchangeReserveGetResponseDto>, CurrencyExchangeReserveCrudGetAllServiceError>;
using CurrencyExchangeReserveResult = Result<CurrencyExchangeReserveCreateResponseDto, CurrencyExchangeReserveCrudCreateServiceError>;
public class CurrencyExchangeReserveCrudService
{
    private readonly ApplicationDbContext _dbContext;
    public CurrencyExchangeReserveCrudService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CurrencyExchangeReserveResult> CreateAsync(CurrencyExchangeReserveCreateRequestDto requestDto)
    {
        CurrencyExchangeReserveCreateResponseDto responseDto;

        // Balance must be positive
        if (requestDto.Balance < 0)
        {
            return CurrencyExchangeReserveResult
                .Fail(CurrencyExchangeReserveCrudCreateServiceError.NegativeBalance, "Balance must be positive or zero");
        }

        // FKs just admits positive values
        if (requestDto.CurrencyId < 0)
        {
            return CurrencyExchangeReserveResult.Fail(CurrencyExchangeReserveCrudCreateServiceError.NagativeCurrencyId,
             "CurrencyId must be positive or zero");
        }


        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            bool currencyAlreadyExists = await _dbContext.CurrencyExchangeReserves.AnyAsync(e => e.CurrencyId == requestDto.CurrencyId);

            if (currencyAlreadyExists)
            {
                return CurrencyExchangeReserveResult.Fail(CurrencyExchangeReserveCrudCreateServiceError.CurrencyAlreadyExistsInReserve,
                "Currency already exists in reserve");
            }

            CurrencyExchangeReserve exchangeReserve = new()
            {
                CurrencyId = requestDto.CurrencyId,
                Balance = requestDto.Balance
            };

            await _dbContext.CurrencyExchangeReserves.AddAsync(exchangeReserve);
            await _dbContext.SaveChangesAsync();

            responseDto = new()
            {
                CurrencyExchangeReserveId = exchangeReserve.CurrencyExchangeReserveId,
            };

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        return CurrencyExchangeReserveResult.Success(responseDto);
    }

    public async Task<CurrencyExchangeReserveGetAllResult> GetAllAsync()
    {
        var result = await _dbContext.CurrencyExchangeReserves.Select(e => new CurrencyExchangeReserveGetResponseDto()
        {
            CurrencyExchangeReserveId = e.CurrencyExchangeReserveId,
            CurrencyId = e.CurrencyId,
            Balance = e.Balance
        }).ToListAsync();

        return CurrencyExchangeReserveGetAllResult.Success(result);
    }

}
