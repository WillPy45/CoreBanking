using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Common;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<IEnumerable<CustomerEssentialDTO>> GetCustomerAsync()
        {
            return await _dbContext.Customers
                .Select((customer) => CustomerEssentialDTO.FromEntity(customer))
                .ToListAsync();
        }

        [HttpPost]
        public async Task<CustomerEssentialDTO> InsertCustomerAsync(CustomerDTO customerDto) 
        {
            //Dto to Entity
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
           
            // entity to output dto
            return CustomerEssentialDTO.FromEntity(customer);
        }
    }
}
