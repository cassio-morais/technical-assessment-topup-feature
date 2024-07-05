using System.ComponentModel.DataAnnotations;

namespace Backend.TopUp.Core.Api.Request
{
    public sealed record AddBeneficiaryRequest(
        [Required(ErrorMessage = "{0} is required")]
        [MaxLength(20, ErrorMessage = "{0} maximum 15 characters")]
        string Nickname,
        [Required(ErrorMessage = "{0} is required")]
        [MaxLength(16, ErrorMessage = "{0} maximum 16 characters")]
        string PhoneNumber);
}
