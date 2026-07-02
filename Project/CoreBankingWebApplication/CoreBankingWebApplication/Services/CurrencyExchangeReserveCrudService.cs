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

        public async Task<Result<CurrencyExchangeReserveCreateResponseDto>> CreateCurrencyReserve(CurrencyExchangeReserveCreateRequestDto requestDto)
        {
            CurrencyExchangeReserveCreateResponseDto responseDto;

            // Balance must be positive
            if (requestDto.Balance < 0)
            {
                return Result<CurrencyExchangeReserveCreateResponseDto>
                    .Fail(new Error("BadRequest.BalanceMustBePositiveOrZero", "Balance must be positive or zero"));
            }

            // FKs just admits positive values
            if (requestDto.CurrencyId < 0)
            {
                return Result<CurrencyExchangeReserveCreateResponseDto>
                    .Fail(new Error("BadRequest.CurrencyIdMustBePositiveOrZero", "CurrencyId must be positive or zero"));
            }


            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                bool currencyAlreadyExists = await _dbContext.CurrencyExchangeReserves.AnyAsync(e => e.CurrencyId == requestDto.CurrencyId);
                
                if (currencyAlreadyExists)
                {
                    return Result<CurrencyExchangeReserveCreateResponseDto>
                        .Fail(new Error("Reserve.CurrencyAlreadyExistsInReserve", "Currency already exists in reserve"));
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

            return Result<CurrencyExchangeReserveCreateResponseDto>.Success(responseDto);
        }       
    }
}