using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;

namespace Backend.TopUp.Core.Infrastruture.Repositories
{
    public interface ITopUpTransactionRepository
    {
        Task<Result<List<TopUpTransaction>>> ListTopUpTransactionsByUserIdWithinAPeriodAsync(Guid id, DateTimeOffset startDate, DateTimeOffset endDate);

        Task<Result<Guid>> CreateTopUpTransactionAsync(TopUpTransaction topUpTransaction);

        Task<Result<Guid>> UpdateTopUpTransactionStatusAsync(Guid topUpTransactionId, TopUpTransactionStatus topUpTransactionStatus, string? reason = null);
    }
}
