using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using System.Linq.Expressions;

namespace Backend.TopUp.Core.Infrastruture.Repository
{
    public interface ITopUpRepository
    {
        Task<Result<Guid>> AddTopUpBeneficiaryAsync(TopUpBeneficiary beneficiary);
        Task<Result<List<TopUpBeneficiary>>> ListBeneficiariesAsync(Expression<Func<TopUpBeneficiary, bool>> predicate);
        Task<Result<bool>> BeneficiaryExists(Expression<Func<TopUpBeneficiary, bool>> predicate);
    }
}
