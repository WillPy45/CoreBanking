using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;

using Microsoft.EntityFrameworkCore;

namespace CoreBankingWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public BankAccountController 
            (ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ICollection<BankAccountBasicOutputDTO>> GetBankAccounts()
        {
            return await _dbContext.BankAccounts
                .Select((account) => BankAccountBasicOutputDTO.FromEntity(account))
                .ToListAsync();
        }

        [HttpPost]
        public async Task<BankAccountBasicOutputDTO> Post(BankAccountInputDTO bankAccountInputDTO)
        {
            BankAccount entity = bankAccountInputDTO.ToEntity();

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await _dbContext.BankAccounts.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

            } catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return BankAccountBasicOutputDTO.FromEntity(entity);
        }

        
    }



}
