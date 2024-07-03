using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using System.Linq.Expressions;

namespace Backend.TopUp.Core.Infrastruture.Repositories
{
    public interface ITopUpOptionRepository
    {
        Task<Result<List<TopUpOption>>> ListTopUpOptionsAsync(Expression<Func<TopUpOption, bool>> predicate);
        Task<Result<TopUpOption>> GetTopUpOptionById(Guid id);
    }
}
