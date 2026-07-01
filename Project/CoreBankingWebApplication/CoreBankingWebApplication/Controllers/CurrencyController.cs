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

            return result.Value!;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CurrencyDto dto)
        {
            var currencyCrudServiceResult = await _currencyCrudService.CreateAsync(dto);

            if (currencyCrudServiceResult.IsFailure)
            {   
                /*if (currencyCrudServiceResult.Error is null)
                {
                    throw new Exception("Error code must not be null, service is buggy");
                }*/

                if (currencyCrudServiceResult.Error.Code == "Currency.AlreadyExists")
                {
                    return BadRequest(currencyCrudServiceResult.Error);
                }
            }
            return Created();
        }
    }

}