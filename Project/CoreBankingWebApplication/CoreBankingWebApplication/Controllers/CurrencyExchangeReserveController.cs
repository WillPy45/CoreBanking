using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace CoreBankingWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyExchangeReserveController : ControllerBase
    {
        private readonly CurrencyExchangeReserveCrudService _currencyExchangeCrudService;

        public CurrencyExchangeReserveController(CurrencyExchangeReserveCrudService currencyExchangeReserveCrudService)
        {
            _currencyExchangeCrudService = currencyExchangeReserveCrudService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            
            return StatusCode(StatusCodes.Status200OK, await _currencyExchangeCrudService.GetAllAsync());
        }
    }
}