using Microsoft.AspNetCore.Mvc;

namespace Backend.Account.Api.Controllers
{
    // **THIS CLASS IS JUST A SIMULATION**

    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{Version:apiVersion}/[controller]")]
    public class AccountController : ControllerBase
    {
        /// <summary>
        /// Withdraw from balance by user Id
        /// </summary>
        /// <param name="request"> withdraw from balance request </param>
        /// <response code="204"> </response>
        [HttpPost("balance/withdrawl")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult WithdrawBalanceAsync([FromBody] BalanceWithdrawlRequest request)
        {
            Random rnd = new Random();
            var number = rnd.Next(1, 10);

            if (number <= 2)
                return BadRequest(new ProblemDetails() { Title = "Error during withdraw from balance request" });

            return Ok(new BalanceWithdrawlResponse(Guid.NewGuid()));
        }

        public record BalanceWithdrawlRequest(Guid UserId, decimal Amount, Guid ExternalTransactionId);
        public record BalanceWithdrawlResponse(Guid BalanceWithdrawTransactionId);
    }
}
