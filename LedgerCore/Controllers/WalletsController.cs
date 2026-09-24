using LedgerCore.Application.Dtos.WalletDtos;
using LedgerCore.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LedgerCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly WalletService _walletService;

        public WalletsController(WalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost]
        public async Task<ActionResult<WalletResponse>> CreateWallet([FromBody] CreateWalletRequest request)
        {
            var wallet = await _walletService.CreateWallet(request.UserId, request.Currency);
            var response = WalletResponse.FromDomain(wallet);

            return CreatedAtAction(
               nameof(GetWallet),
               new { accountNumber = wallet.AccountNumber },
               response);
        }

        [HttpGet("{accountNumber}")]
        public async Task<ActionResult<WalletResponse>> GetWallet(string accountNumber)
        {
            var wallet = await _walletService.GetByAccountNumber(accountNumber);
            if (wallet is null)
                return NotFound();

            return Ok(WalletResponse.FromDomain(wallet));
        }

        [HttpPost("{accountNumber}/disable")]
        public async Task<IActionResult> Disable(string accountNumber, [FromBody] WalletActionRequest request)
        {
            await _walletService.DisableWallet(accountNumber, request.ActingUserId, request.Reason);
            return NoContent();
        }

        [HttpPost("{accountNumber}/enable")]
        public async Task<IActionResult> Enable(string accountNumber, [FromBody] WalletActionRequest request)
        {
            await _walletService.EnableWallet(accountNumber, request.ActingUserId, request.Reason);
            return NoContent();
        }

        [HttpPost("{accountNumber}/request-closure")]
        public async Task<IActionResult> RequestClosure(string accountNumber, [FromBody] WalletActionRequest request)
        {
            await _walletService.RequestClosure(accountNumber, request.ActingUserId, request.Reason);
            return NoContent();
        }

        [HttpPost("{accountNumber}/approve-closure")]
        public async Task<IActionResult> ApproveClosure(string accountNumber, [FromBody] WalletActionRequest request)
        {
            await _walletService.ApproveClosure(accountNumber, request.ActingUserId, request.Reason);
            return NoContent();
        }
    }
}
