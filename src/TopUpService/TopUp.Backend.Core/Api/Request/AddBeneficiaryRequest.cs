using System.ComponentModel.DataAnnotations;

namespace Backend.TopUp.Core.Api.Request
{
    public record AddBeneficiaryRequest(
        [Required(ErrorMessage = "{0} can't not be null")]
        [MaxLength(20, ErrorMessage = "{0} maximum 15 characters")] 
        string Nickname, 
        [Required(ErrorMessage = "{0} can't not be null")]
        [MaxLength(15, ErrorMessage = "{0} maximum 15 characters")]
        string PhoneNumber);
}
