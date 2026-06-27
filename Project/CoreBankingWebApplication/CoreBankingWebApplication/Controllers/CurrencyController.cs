using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Services;

using Microsoft.EntityFrameworkCore;


namespace CoreBankingWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyCrudService _currencyCrudService;

        public CurrencyController(CurrencyCrudService currencyCrudService)
        {
            _currencyCrudService = currencyCrudService;
        }

        [HttpGet]
        public async Task<ICollection<CurrencyBasicOutputDto>> GetCurrencies()
        {
            var result = await _currencyCrudService.GetCurrencies();

            if (result.IsFailure)
            {
                // TO DO LATER
            }

            return result.Value;
        }
    }

}