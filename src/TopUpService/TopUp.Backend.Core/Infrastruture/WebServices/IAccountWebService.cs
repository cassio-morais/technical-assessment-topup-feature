using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Infrastruture.WebServices.Response;

namespace Backend.TopUp.Core.Infrastruture.WebServices
{
    public interface IBankAccountWebService
    {
        Task<Result<BalanceWithdrawlResponse>> WithdrawBalanceAsync(Guid userId, decimal amount, Guid externalTransactionId);
    }
}
