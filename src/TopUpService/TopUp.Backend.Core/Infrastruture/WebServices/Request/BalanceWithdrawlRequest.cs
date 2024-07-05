namespace Backend.TopUp.Core.Infrastruture.WebServices.Request
{
    public record BalanceWithdrawlRequest(Guid UserId, decimal Amount, Guid ExternalTransactionId);
}
