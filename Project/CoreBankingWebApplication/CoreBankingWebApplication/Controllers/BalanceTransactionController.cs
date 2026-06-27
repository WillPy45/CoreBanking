using Microsoft.AspNetCore.Mvc;

using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Models;
using CoreBankingWebApplication.Services;

using Microsoft.EntityFrameworkCore;
using CoreBankingWebApplication.Common;

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
        public async Task<Result<string>> Post(BalanceTransactionDTO balanceTransactionDTO)
        {
            int senderID = balanceTransactionDTO.SenderBankAccountId;
            int receiverID = balanceTransactionDTO.ReceiverBankAccountId;
            decimal amount = balanceTransactionDTO.Amount;

            var serviceResult = await _transferService.TransferAsync(balanceTransactionDTO);



            return serviceResult;    
        }
    } 
}