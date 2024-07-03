using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using System.Linq.Expressions;

namespace Backend.TopUp.Core.Infrastruture.Repositories
{
    public interface ITopUpBeneficiaryRepository
    {
        Task<Result<Guid>> AddTopUpBeneficiaryAsync(TopUpBeneficiary beneficiary);
        Task<Result<List<TopUpBeneficiary>>> ListTopUpBeneficiariesAsync(Expression<Func<TopUpBeneficiary, bool>> predicate);
        Task<Result<bool>> TopUpBeneficiaryExists(Expression<Func<TopUpBeneficiary, bool>> predicate);
        Task<Result<TopUpBeneficiary>> GetTopUpBeneficiaryById(Guid id);
    }
}
