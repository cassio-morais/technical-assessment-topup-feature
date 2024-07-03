using System.Data;

namespace Backend.TopUp.Core.Entities
{
    public sealed class TopUpTransaction : BaseEntity<Guid>
    {
        public TopUpTransaction(Guid userId, Guid topUpBeneficiaryId, decimal amount, DateTimeOffset transactionDate)
            : base(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), null)
        {
            UserId = userId;
            TopUpBeneficiaryId = topUpBeneficiaryId;
            Amount = amount;
            TransactionDate = transactionDate;
        }

        public static TopUpTransaction NewPendingTransaction(Guid userId, Guid topUpBeneficiaryId, decimal amount) 
        {
            return new TopUpTransaction(userId, topUpBeneficiaryId, amount, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc));
        }

        public Guid UserId { get; private set; }
        public Guid TopUpBeneficiaryId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTimeOffset TransactionDate { get; private set; }
        public TopUpBeneficiary? ToUpBeneficiary { get; private set; }
        public TopUpTransactionStatus? Status { get; private set; }
        public string? Reason { get; private set; }

        public const decimal TransactionCharge = 1;

        public void UpdateStatus(TopUpTransactionStatus topUpTransactionStatus, string? reason = null) 
        {
            Status = topUpTransactionStatus;
            Reason = reason;
        }
    }

    public enum TopUpTransactionStatus
    {
        Pending = 1,
        Success,
        Error
    }
}
