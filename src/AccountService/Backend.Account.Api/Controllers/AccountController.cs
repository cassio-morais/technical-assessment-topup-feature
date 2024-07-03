using Microsoft.AspNetCore.Mvc;

namespace Backend.Account.Api.Controllers
{
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
        [HttpPost("balance")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult WithdrawFromBalanceAsync([FromBody] WithdrawFromBalanceRequest request)
        {
            Random rnd = new Random();
            var number = rnd.Next(1, 10);

            if (number <= 2)
                return BadRequest(new ProblemDetails() { Title = "Error during withdraw from balance request" });

            return NoContent();
        }

        public record WithdrawFromBalanceRequest(Guid UserId, decimal Amount, Guid ExternalTransactionId);
    }
}
