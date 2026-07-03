using Microsoft.AspNetCore.Mvc;

using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models.Entities;
using CoreBankingWebApplication.Models.DTOs;
using CoreBankingWebApplication.Services;


using Microsoft.EntityFrameworkCore;
using CoreBankingWebApplication.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CoreBankingWebApplication.Controllers
{
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
                        return Conflict(serviceResult.Error.Message);

                    case TransferBalanceBetweenAccountError.NegativeOrZeroAmount:
                    case TransferBalanceBetweenAccountError.SameBankAccount:
                        return BadRequest(serviceResult.Error.Code);

                    case TransferBalanceBetweenAccountError.ReceiverBankccountNotFound:
                    case TransferBalanceBetweenAccountError.SenderBankAccountNotFound:
                        return NotFound(serviceResult.Error.Code);

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, serviceResult.Error.Code);   
                }
            }
            return Ok(serviceResult.Value);    
        }
    } 

    
}