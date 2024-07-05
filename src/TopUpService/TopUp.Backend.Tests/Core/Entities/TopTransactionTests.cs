using Backend.TopUp.Core.Entities;
using FluentAssertions;

namespace Backend.TopUp.Tests.Core.Entities
{
    public class TopUpTransactionTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var topUpBeneficiaryId = Guid.NewGuid();
            var amount = 100m;
            var transactionDate = DateTimeOffset.UtcNow;
            var transactionCost = 1m;

            // Act
            var topUpTransaction = new TopUpTransaction(userId, topUpBeneficiaryId, amount, transactionDate, transactionCost);

            // Assert
            topUpTransaction.UserId.Should().Be(userId);
            topUpTransaction.TopUpBeneficiaryId.Should().Be(topUpBeneficiaryId);
            topUpTransaction.Amount.Should().Be(amount);
            topUpTransaction.TransactionDate.Should().Be(transactionDate);
            topUpTransaction.TransactionCost.Should().Be(transactionCost);
            topUpTransaction.Status.Should().BeNull();
            topUpTransaction.Reason.Should().BeNull();
        }

        [Fact]
        public void NewPendingTransaction_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var topUpBeneficiaryId = Guid.NewGuid();
            var amount = 100m;
            var transactionCost = 1m;
            var beforeCreationTime = DateTimeOffset.UtcNow;

            // Act
            var topUpTransaction = TopUpTransaction.NewPendingTransaction(userId, topUpBeneficiaryId, amount, transactionCost);
            var afterCreationTime = DateTimeOffset.UtcNow;

            // Assert
            topUpTransaction.UserId.Should().Be(userId);
            topUpTransaction.TopUpBeneficiaryId.Should().Be(topUpBeneficiaryId);
            topUpTransaction.Amount.Should().Be(amount);
            topUpTransaction.TransactionCost.Should().Be(transactionCost);
            topUpTransaction.Status.Should().BeNull();
            topUpTransaction.Reason.Should().BeNull();
        }

        [Fact]
        public void UpdateStatus_ShouldSetStatusAndReason()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var topUpBeneficiaryId = Guid.NewGuid();
            var amount = 100m;
            var transactionDate = DateTimeOffset.UtcNow;
            var transactionCost = 1m;
            var topUpTransaction = new TopUpTransaction(userId, topUpBeneficiaryId, amount, transactionDate, transactionCost);
            var newStatus = TopUpTransactionStatus.Success;
            var reason = "Transaction completed successfully";

            // Act
            topUpTransaction.UpdateStatus(newStatus, reason);

            // Assert
            topUpTransaction.Status.Should().Be(newStatus);
            topUpTransaction.Reason.Should().Be(reason);
        }

        [Fact]
        public void UpdateStatus_ShouldSetStatusWithoutReason()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var topUpBeneficiaryId = Guid.NewGuid();
            var amount = 100m;
            var transactionDate = DateTimeOffset.UtcNow;
            var transactionCost = 1m;
            var topUpTransaction = new TopUpTransaction(userId, topUpBeneficiaryId, amount, transactionDate, transactionCost);
            var newStatus = TopUpTransactionStatus.Pending;

            // Act
            topUpTransaction.UpdateStatus(newStatus);

            // Assert
            topUpTransaction.Status.Should().Be(newStatus);
            topUpTransaction.Reason.Should().BeNull();
        }
    }
}
