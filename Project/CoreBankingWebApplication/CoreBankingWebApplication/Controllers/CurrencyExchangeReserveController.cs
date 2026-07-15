using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreBankingWebApplication.Controllers;

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
        var serviceResult = await _currencyExchangeCrudService.GetAllAsync();
        return StatusCode(StatusCodes.Status200OK, serviceResult.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CurrencyExchangeReserveCreateRequestDto requestDto)
    {
        var serviceResult = await _currencyExchangeCrudService.CreateAsync(requestDto);

        if (serviceResult.IsFailure)
        {
            switch (serviceResult.Error.Code)
            {
                case CurrencyExchangeReserveCrudCreateServiceError.CurrencyAlreadyExistsInReserve:
                    return Conflict(serviceResult.Error.Message);

                case CurrencyExchangeReserveCrudCreateServiceError.NagativeCurrencyId:
                case CurrencyExchangeReserveCrudCreateServiceError.NegativeBalance:
                    return BadRequest(serviceResult.Error.Message);
            }
        }

        return Ok(serviceResult.Value);
    }
}
