using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;

namespace CoreBankingWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerController
            (ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<CustomerEssentialDTO> InsertCustomer(CustomerDTO customerDto) 
        {
            //Mapear DTO a Entidad
            Customer customer = customerDto.ToEntity();

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {

                await _dbContext.Customers.AddAsync(customer);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }catch (Exception exception)
            {
                await transaction.RollbackAsync();
                throw exception;
            }
           
            // Mapear Entidad a DTO de salida
            return CustomerEssentialDTO.FromEntity(customer);
        }
    }
}
