using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;

namespace Backend.TopUp.Core.Infrastruture.Repositories
{
    public interface ITopUpTransactionRepository
    {
        Task<Result<List<TopUpTransaction>>> ListTopUpTransactionsByUserIdWithinAPeriod(Guid id, DateTimeOffset startDate, DateTimeOffset endDate);

        Task<Result<Guid>> CreateTopUpTransaction(TopUpTransaction topUpTransaction);

        Task<Result<Guid>> UpdateTopUpTransactionStatus(Guid topUpTransactionId, TopUpTransactionStatus topUpTransactionStatus, string? reason = null);
    }
}
