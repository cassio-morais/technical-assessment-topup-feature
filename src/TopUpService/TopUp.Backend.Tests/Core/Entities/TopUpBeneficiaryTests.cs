using Backend.TopUp.Core.Entities;
using FluentAssertions;

namespace Backend.TopUp.Tests.Core.Entities
{
    public class TopUpBeneficiaryTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var nickname = "ValidNick";
            var phoneNumber = "+971-50-1234567";
            var isActive = true;

            // Act
            var topUpBeneficiary = new TopUpBeneficiary(userId, nickname, phoneNumber, isActive);

            // Assert
            topUpBeneficiary.UserId.Should().Be(userId);
            topUpBeneficiary.Nickname.Should().Be(nickname);
            topUpBeneficiary.PhoneNumber.Should().Be(phoneNumber);
            topUpBeneficiary.IsActive.Should().Be(isActive);
        }

        [Fact]
        public void Constructor_Nickname_ShouldThrowException_WhenNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var phoneNumber = "+971-50-1234567";
            var isActive = true;

            // Act
            Action act = () => new TopUpBeneficiary(userId, null, phoneNumber, isActive);

            // Assert
            act.Should().Throw<Exception>().WithMessage("nickname can´t not be null or greater than 20 characters");
        }

        [Fact]
        public void Constructor_Nickname_ShouldThrowException_WhenExceedsMaxLength()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var nickname = new string('a', 21);
            var phoneNumber = "+971-50-1234567";
            var isActive = true;

            // Act
            Action act = () => new TopUpBeneficiary(userId, nickname, phoneNumber, isActive);

            // Assert
            act.Should().Throw<Exception>().WithMessage("nickname can´t not be null or greater than 20 characters");
        }

        [Fact]
        public void Constructor_PhoneNumber_ShouldThrowException_WhenInvalid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var nickname = "ValidNick";
            var invalidPhoneNumber = "+123-654654000";
            var isActive = true;

            // Act
            Action act = () => new TopUpBeneficiary(userId, nickname, invalidPhoneNumber, isActive);

            // Assert
            act.Should().Throw<Exception>().WithMessage("Invalid UAE phone number");
        }

        [Fact]
        public void Constructor_PhoneNumber_ShouldThrowException_WhenNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var nickname = "ValidNick";
            var isActive = true;

            // Act
            Action act = () => new TopUpBeneficiary(userId, nickname, null, isActive);

            // Assert
            act.Should().Throw<Exception>().WithMessage("Invalid UAE phone number");
        }

        [Fact]
        public void Constructor_PhoneNumber_ShouldThrowException_WhenEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var nickname = "ValidNick";
            var isActive = true;

            // Act
            Action act = () => new TopUpBeneficiary(userId, nickname, string.Empty, isActive);

            // Assert
            act.Should().Throw<Exception>().WithMessage("Invalid UAE phone number");
        }

        [Fact]
        public void Constructor_IsActive_ShouldBeSetCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var nickname = "ValidNick";
            var phoneNumber = "+971-50-1234567";
            var isActive = true;

            // Act
            var topUpBeneficiary = new TopUpBeneficiary(userId, nickname, phoneNumber, isActive);

            // Assert
            topUpBeneficiary.IsActive.Should().BeTrue();
        }
    }
}
