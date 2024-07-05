using Backend.TopUp.Core.Application;
using FluentAssertions;

namespace Backend.TopUp.Tests.Core.Services
{
    public class TopUpRulesTests
    {
        [Fact]
        public void HasTheUserReachedTheTopUpLimitThisMonthForThisBeneficiary_ShouldReturnTrue_WhenUnverifiedUserExceedsLimit()
        {
            // Arrange
            decimal totalAmountForTheMonthPerBeneficiary = 450;
            decimal topUpValue = 100;
            bool isUserVerified = false;

            // Act
            bool result = TopUpRules.HasTheUserReachedTheTopUpLimitThisMonthForThisBeneficiary(totalAmountForTheMonthPerBeneficiary, topUpValue, isUserVerified);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasTheUserReachedTheTopUpLimitThisMonthForThisBeneficiary_ShouldReturnFalse_WhenUnverifiedUserWithinLimit()
        {
            // Arrange
            decimal totalAmountForTheMonthPerBeneficiary = 400;
            decimal topUpValue = 50;
            bool isUserVerified = false;

            // Act
            bool result = TopUpRules.HasTheUserReachedTheTopUpLimitThisMonthForThisBeneficiary(totalAmountForTheMonthPerBeneficiary, topUpValue, isUserVerified);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasTheUserReachedTheTopUpLimitThisMonthForThisBeneficiary_ShouldReturnTrue_WhenVerifiedUserExceedsLimit()
        {
            // Arrange
            decimal totalAmountForTheMonthPerBeneficiary = 950;
            decimal topUpValue = 100;
            bool isUserVerified = true;

            // Act
            bool result = TopUpRules.HasTheUserReachedTheTopUpLimitThisMonthForThisBeneficiary(totalAmountForTheMonthPerBeneficiary, topUpValue, isUserVerified);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasTheUserReachedTheTopUpLimitThisMonthForThisBeneficiary_ShouldReturnFalse_WhenVerifiedUserWithinLimit()
        {
            // Arrange
            decimal totalAmountForTheMonthPerBeneficiary = 900;
            decimal topUpValue = 50;
            bool isUserVerified = true;

            // Act
            bool result = TopUpRules.HasTheUserReachedTheTopUpLimitThisMonthForThisBeneficiary(totalAmountForTheMonthPerBeneficiary, topUpValue, isUserVerified);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasTheUserReachedTheTopUpLimitThisMonth_ShouldReturnTrue_WhenUserExceedsLimit()
        {
            // Arrange
            decimal totalAmountForTheMonth = 2900;
            decimal topUpValue = 200;

            // Act
            bool result = TopUpRules.HasTheUserReachedTheTopUpLimitThisMonth(totalAmountForTheMonth, topUpValue);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasTheUserReachedTheTopUpLimitThisMonth_ShouldReturnFalse_WhenUserWithinLimit()
        {
            // Arrange
            decimal totalAmountForTheMonth = 2500;
            decimal topUpValue = 400;

            // Act
            bool result = TopUpRules.HasTheUserReachedTheTopUpLimitThisMonth(totalAmountForTheMonth, topUpValue);

            // Assert
            result.Should().BeFalse();
        }
    }
}
