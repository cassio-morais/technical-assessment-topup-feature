using Backend.TopUp.Core.Api.Request;
using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;

namespace Backend.TopUp.Core.Application.Services
{
    public interface ITopUpService
    {
        Task<Result<Guid>> AddTopUpBeneficiaryAsync(Guid userId, AddBeneficiaryRequest request);
        Task<Result<List<TopUpBeneficiary>>> ListBeneficiariesByUserIdAsync(Guid userId);
        Task<Result<List<TopUpOption>>> ListTopUpOptionsByUserIdAsync(Guid userId, string currencyAbbreviation);
        Task<Result<Guid>> RequestTopUpByUserIdAsync(Guid userId, TopUpRequest request);
    }
}
