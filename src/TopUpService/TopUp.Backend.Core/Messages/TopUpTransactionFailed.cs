namespace Backend.TopUp.Core.Messages
{
    public record TopUpTransactionFailed(Guid UserId, decimal Amount, Guid BalanceWithdrawTransactioniId, Guid ExternalTransactionId);

}
