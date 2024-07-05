using Backend.TopUp.Core.Entities;
using FluentAssertions;

namespace Backend.TopUp.Tests.Core.Entities
{
    public class TopUpOptionTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var currencyAbbreviation = "AED";
            var value = 100m;
            var isActive = true;

            // Act
            var topUpOption = new TopUpOption(currencyAbbreviation, value, isActive);

            // Assert
            topUpOption.CurrencyAbbreviation.Should().Be(currencyAbbreviation);
            topUpOption.Value.Should().Be(value);
            topUpOption.IsActive.Should().Be(isActive);
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenCurrencyAbbreviationIsNull()
        {
            // Arrange
            string currencyAbbreviation = null;
            var value = 100m;
            var isActive = true;

            // Act
            Action act = () => new TopUpOption(currencyAbbreviation, value, isActive);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Currency abbreviation cannot be null or empty.");
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenCurrencyAbbreviationIsEmpty()
        {
            // Arrange
            var currencyAbbreviation = "";
            var value = 100m;
            var isActive = true;

            // Act
            Action act = () => new TopUpOption(currencyAbbreviation, value, isActive);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Currency abbreviation cannot be null or empty.");
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenValueIsNegative()
        {
            // Arrange
            var currencyAbbreviation = "AED";
            var value = -100m;
            var isActive = true;

            // Act
            Action act = () => new TopUpOption(currencyAbbreviation, value, isActive);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Value cannot be negative. (Parameter 'value')");
        }
    }
}
