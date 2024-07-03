using Backend.TopUp.Core.Contracts;

namespace Backend.TopUp.Core.Infrastruture.WebServices
{
    public interface IBankAccountWebService
    {
        Task<Result<bool>> WithdrawFromBalance(Guid userId, decimal amount, Guid externalTransactionId);
    }
}
