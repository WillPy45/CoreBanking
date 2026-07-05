using Microsoft.AspNetCore.Mvc;

using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Services;


using Microsoft.EntityFrameworkCore;
using CoreBankingWebApplication.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CoreBankingWebApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BalanceTransactionController : ControllerBase
{
    private readonly TransferBalanceBetweenAccountService _transferService;

    public BalanceTransactionController(
        TransferBalanceBetweenAccountService transferBalanceService)
    {
        _transferService = transferBalanceService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(BalanceTransactionDTO balanceTransactionDTO)
    {
        var serviceResult = await _transferService.TransferAsync(balanceTransactionDTO);
        if (serviceResult.IsFailure)
        {
            switch (serviceResult.Error.Code)
            {
                case TransferBalanceBetweenAccountError.CurrencyDiffers:
                case TransferBalanceBetweenAccountError.NotEnoughBalance:
                    return Conflict(serviceResult.Error.Message);

                case TransferBalanceBetweenAccountError.NegativeOrZeroAmount:
                case TransferBalanceBetweenAccountError.SameBankAccount:
                    return BadRequest(serviceResult.Error.Message);

                case TransferBalanceBetweenAccountError.ReceiverBankccountNotFound:
                case TransferBalanceBetweenAccountError.SenderBankAccountNotFound:
                    return NotFound(serviceResult.Error.Message);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, serviceResult.Error.Message);
            }
        }
        return Ok(serviceResult.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var serviceResult = await _transferService.GetAllTransactionAsync();
        if (serviceResult.IsFailure)
        {

        }

        return StatusCode(StatusCodes.Status200OK, serviceResult.Value);
    }
}


