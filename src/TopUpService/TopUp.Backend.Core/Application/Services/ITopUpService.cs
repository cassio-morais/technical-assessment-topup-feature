using Backend.TopUp.Core.Api.Request;
using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;

namespace Backend.TopUp.Core.Application.Services
{
    public interface ITopUpService
    {
        Task<Result<Guid>> AddTopUpBeneficiaryAsync(Guid userId, AddBeneficiaryRequest beneficiary);
        Task<Result<List<TopUpBeneficiary>>> ListBeneficiariesByUserId(Guid userId);
    }
}
