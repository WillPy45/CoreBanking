using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;

using CoreBankingWebApplication.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.AspNetCore.Http.HttpResults;
using CoreBankingWebApplication.Common;


namespace CoreBankingWebApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CurrencyExchangeController : ControllerBase
{
    private readonly CurrencyExchangeService _currencyExchangeService;
    public CurrencyExchangeController(CurrencyExchangeService currencyExchangeService)
    {
        _currencyExchangeService = currencyExchangeService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(CurrencyExchangeRequestDto requestDto)
    {
        var serviceResult = await _currencyExchangeService.ExchangeAsync(requestDto);

        if (serviceResult.IsFailure)
        {
            switch (serviceResult.Error.Code)
            {
                case CurrencyExchangeServiceError.SameBankAccounts:
                case CurrencyExchangeServiceError.NegativeOrZeroAmount:
                    return BadRequest(serviceResult.Error.Message);

                case CurrencyExchangeServiceError.SenderBankAccountNotFound:
                case CurrencyExchangeServiceError.ReceiverBankAccountNotFound:
                    return NotFound(serviceResult.Error.Message);

                case CurrencyExchangeServiceError.NotEnoughBalance:
                case CurrencyExchangeServiceError.HasNotEnoughBalanceInCurrencyReserve:
                case CurrencyExchangeServiceError.HasNotReserveForDepositTheCurrencies:
                case CurrencyExchangeServiceError.HasNotReserveForAcceptTheCurrencies:
                case CurrencyExchangeServiceError.SameCurrencyType:
                    return Conflict(serviceResult.Error.Message);

                case CurrencyExchangeServiceError.ConversionFailed:
                    return StatusCode(StatusCodes.Status424FailedDependency, serviceResult.Error.Message);
            }
        }

        return Ok(serviceResult.Value);

    }
}
