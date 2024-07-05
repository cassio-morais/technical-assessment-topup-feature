using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Infrastruture.WebServices;
using Backend.TopUp.Core.Infrastruture.WebServices.Request;
using Backend.TopUp.Core.Infrastruture.WebServices.Response;
using Refit;
using System.Net.Http.Json;


namespace Backend.TopUp.Infrastructure.WebServices
{
    public class BankAccountWebService(IBankAccountApi bankAccountApi) : IBankAccountWebService
    {
        private readonly IBankAccountApi _bankAccountApi = bankAccountApi;
        public async Task<Result<BalanceWithdrawlResponse>> WithdrawBalanceAsync(Guid userId, decimal amount, Guid externalTransactionId)
        {
            var response = await _bankAccountApi.WithdrawBalanceAsync(new BalanceWithdrawlRequest(userId, amount, externalTransactionId));
            if (!response.IsSuccessStatusCode)
                return Result<BalanceWithdrawlResponse>.Error("some error has ocurred");

            var balanceWithdrawl = await response!.Content!.ReadFromJsonAsync<BalanceWithdrawlResponse>();

            return Result<BalanceWithdrawlResponse>.Ok(balanceWithdrawl!);
        }
    }

    // Using refit just for simplicity...
    // in real world many others conditions need to be analyzed to choose the http/rest lib...
    // Moreover, retry policies are welcome.
    public interface IBankAccountApi 
    {
        [Post("/api/v1/Account/balance/withdrawl")]
        Task<HttpResponseMessage> WithdrawBalanceAsync(BalanceWithdrawlRequest request);
    }
}
