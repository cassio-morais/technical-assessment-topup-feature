using System.ComponentModel.DataAnnotations;

namespace Backend.TopUp.Core.Api.Request
{
    public sealed record TopUpRequest(
            [Required(ErrorMessage = "{0} is required")]
            Guid BeneficiaryId,
            [Required(ErrorMessage = "{0} is required")]
            Guid TopOptionId);
}
