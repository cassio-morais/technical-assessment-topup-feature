using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Infrastruture.WebServices;
using Refit;

namespace Backend.TopUp.Infrastructure.WebServices
{
    public class BankAccountWebService(IBankAccountApi bankAccountApi) : IBankAccountWebService
    {
        private readonly IBankAccountApi _bankAccountApi = bankAccountApi;

        public async Task<Result<bool>> WithdrawFromBalance(Guid userId, decimal amount, Guid externalTransactionId)
        {
            var response = await _bankAccountApi.WithdrawFromBalance(new WithdrawFromBalanceRequest(userId, amount, externalTransactionId));

            if (!response.IsSuccessStatusCode)
                return Result<bool>.Error("some error has ocurred");

            return Result<bool>.Ok(true);
        }
    }

    // Using refit just for simplicity...
    // in real world many others conditions need to be analyze to choose the http/rest lib...
    // Moreover, retry policies are welcome.
    public interface IBankAccountApi 
    {
        [Post("/api/v1/Account/balance")]
        Task<HttpResponseMessage> WithdrawFromBalance(WithdrawFromBalanceRequest request);
    }

    public record WithdrawFromBalanceRequest(Guid UserId, decimal Amount, Guid ExternalTransactionId);
}
