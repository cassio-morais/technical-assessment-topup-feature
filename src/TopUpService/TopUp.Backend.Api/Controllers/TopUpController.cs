using Backend.TopUp.Api.Helpers;
using Backend.TopUp.Core.Api.Request;
using Backend.TopUp.Core.Api.Response;
using Backend.TopUp.Core.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend.TopUp.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route(GlobalPathHelper.Pattern)]
    public class TopUpController : ControllerBase
    {
        // todo: improve controller documentation
        // todo: put some log here

        private readonly ITopUpService _topUpService;
        public TopUpController(ITopUpService topUpService)
        {
            _topUpService = topUpService;
        }

        /// <summary>
        /// Add beneficiary to an user
        /// </summary>
        /// <remarks>
        ///  UserId's: C30CF3C7-C738-435D-AC77-FA19B6018924 (verified), 29C0F3B9-75C1-4A80-8530-BA295A612B67 (unverified)  <br /> 
        ///  Phone Number example: +971-04-1234567  <br /> 
        ///  Nickname example: John Doe Phone
        /// </remarks>
        /// <param name="userId">User Id [real world this information could be retrieved from a JWT token]</param>
        /// <param name="request"> Beneficiary's phone number and nickname </param>
        /// <response code="204"> </response>
        [HttpPost("users/{userId}/beneficiaries")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddBeneficiaryAsync([Required] Guid userId, [FromBody] AddBeneficiaryRequest request)
        {
            var result = await _topUpService.AddTopUpBeneficiaryAsync(userId,request);

            if (result.HasError)
                return BadRequest(new ProblemDetails() { Title = result.ErrorMessage });
            
            return NoContent();
        }


        /// <summary>
        /// Show beneficiaries by user id
        /// </summary>
        /// <remarks>
        ///  UserId's: C30CF3C7-C738-435D-AC77-FA19B6018924 (verified), 29C0F3B9-75C1-4A80-8530-BA295A612B67 (unverified)
        /// </remarks>
        /// <param name="userId">User Id [real world this information could be retrieved from a JWT token]</param>
        /// <response code="204"> </response>
        [HttpGet("users/{userId}/beneficiaries")]
        [ProducesResponseType(typeof(TopUpBeneficiaryResponse),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ListBeneficiariesAsync([Required] Guid userId)
        {
            var result = await _topUpService.ListBeneficiariesByUserIdAsync(userId);

            if (result.HasError)
                return BadRequest(new ProblemDetails() { Title = result.ErrorMessage });

            var response = result.Data!
                .Select(x => new TopUpBeneficiaryResponse(x.Id, x.Nickname!, x.PhoneNumber!, x.IsActive))
                .ToList();

            return Ok(response);
        }

        /// <summary>
        /// Show top-up options
        /// </summary>
        /// <remarks>
        ///  UserId's: C30CF3C7-C738-435D-AC77-FA19B6018924 (verified), 29C0F3B9-75C1-4A80-8530-BA295A612B67 (unverified)
        /// </remarks>
        /// <param name="userId">User Id [real world this information could be retrieved from a JWT token]</param>
        /// <param name="currencyAbbreviation"> use AED for this test </param>
        /// <response code="204"> </response>
        [HttpGet("users/{userId}/top-up")]
        [ProducesResponseType(typeof(List<TopUpOptionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ListTopUpOptionsAsync([Required] Guid userId,
            [FromQuery(Name = "currency")][Required] string currencyAbbreviation)
        {
            var result = await _topUpService.ListTopUpOptionsByUserIdAsync(userId, currencyAbbreviation);

            if (result.HasError)
                return BadRequest(new ProblemDetails() { Title = result.ErrorMessage });

            var response = result.Data!
                .Select(x => new TopUpOptionResponse(x.Id, x.CurrencyAbbreviation!, x.Value!))
                .OrderBy(x => x.Value)
                .ToList();

            return Ok(response);
        }

        /// <summary>
        /// Order a top-up 
        /// </summary>
        /// <remarks>
        ///  UserId's: C30CF3C7-C738-435D-AC77-FA19B6018924 (verified), 29C0F3B9-75C1-4A80-8530-BA295A612B67 (unverified)
        /// </remarks>
        /// <param name="userId">User Id [real world this information could be retrieved from a JWT token]</param>
        /// <param name="request"> beneficiary Id and top-up option Id </param>
        /// <response code="204"> </response>
        [HttpPost("users/{userId}/top-up")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestTopUpAsync([Required] Guid userId, [FromBody] TopUpRequest request)
        {
            var result = await _topUpService.RequestTopUpByUserIdAsync(userId, request);

            if (result.HasError)
                return BadRequest(new ProblemDetails() { Title = result.ErrorMessage });

            return Ok(new { TransactionId = result.Data });
        }
    }
}
