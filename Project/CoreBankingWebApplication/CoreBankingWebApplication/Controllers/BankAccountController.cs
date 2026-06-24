using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;

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

        [HttpPost]
        public async Task<BankAccountBasicOutputDTO> Post(BankAccountInputDTO bankAccountInputDTO)
        {
            BankAccount entity = bankAccountInputDTO.ToEntity();

            var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await _dbContext.BankAccounts.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

            } catch (Exception exception)
            {
                await transaction.RollbackAsync();
            }
        }
    }



}
