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
        // TODO: improve controller documentation

        private readonly ITopUpService _topUpService;
        public TopUpController(ITopUpService topUpService)
        {
            _topUpService = topUpService;
        }

        /// <summary>
        /// Add beneficiary to an user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="AddBeneficiaryRequest"> Beneficiary's phone number and nickname </param>
        /// <response code="204"> </response>
        [HttpPost("users/{userId}/beneficiaries")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddBeneficiaryAsync([Required] Guid userId, [FromBody] AddBeneficiaryRequest request)
        {
            // this endpoint could have a bulk operation with many beneficiaries, for example
            var result = await _topUpService.AddTopUpBeneficiaryAsync(userId,request);

            if (result.HasError)
                return BadRequest(new BadRequestObjectResult(result.ErrorMessage));
            
            return NoContent();
        }


        /// <summary>
        /// Show beneficiaries by user id
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <response code="204"> </response>
        [HttpGet("users/{userId}/beneficiaries")]
        [ProducesResponseType(typeof(TopUpBeneficiaryResponse),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ListBeneficiariesAsync([Required] Guid userId)
        {
            var result = await _topUpService.ListBeneficiariesByUserId(userId);

            if (result.HasError)
                return BadRequest(new BadRequestObjectResult(result.ErrorMessage));

            var response = result.Data!
                .Select(x => new TopUpBeneficiaryResponse(x.Id, x.Nickname!, x.PhoneNumber!, x.IsActive))
                .ToList();

            return Ok(response);
        }
    }
}
