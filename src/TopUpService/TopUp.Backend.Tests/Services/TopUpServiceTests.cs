using Backend.TopUp.Application.Services;
using Backend.TopUp.Core.Api.Request;
using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using Backend.TopUp.Core.Extensions;
using Backend.TopUp.Core.Infrastruture.Repositories;
using Backend.TopUp.Core.Infrastruture.WebServices;
using Backend.TopUp.Core.Infrastruture.WebServices.Response;
using FluentAssertions;
using MassTransit;
using Moq;
using System.Linq.Expressions;

namespace Backend.TopUp.Tests.Services;

public class TopUpServiceTests
{
    private readonly Mock<ITopUpBeneficiaryRepository> _beneficiaryRepositoryMock;
    private readonly Mock<ITopUpOptionRepository> _topUpOptionRepositoryMock;
    private readonly Mock<ITopUpTransactionRepository> _topUpTransactionRepositoryMock;
    private readonly Mock<IUserWebService> _userWebServiceMock;
    private readonly Mock<IBankAccountWebService> _accountWebServiceMock;
    private readonly Mock<IDateTimeExtensions> _dateTimeExtensionsMock;
    private readonly Mock<IBus> _busMock;

    public TopUpServiceTests()
    {
        _beneficiaryRepositoryMock = new Mock<ITopUpBeneficiaryRepository>();
        _topUpOptionRepositoryMock = new Mock<ITopUpOptionRepository>();
        _topUpTransactionRepositoryMock = new Mock<ITopUpTransactionRepository>();
        _userWebServiceMock = new Mock<IUserWebService>();
        _accountWebServiceMock = new Mock<IBankAccountWebService>();
        _dateTimeExtensionsMock = new Mock<IDateTimeExtensions>();
        _busMock = new Mock<IBus>();
    }

    private TopUpService CreateService()
    {
        return new TopUpService(
            _beneficiaryRepositoryMock.Object,
            _topUpOptionRepositoryMock.Object,
            _topUpTransactionRepositoryMock.Object,
            _userWebServiceMock.Object,
            _accountWebServiceMock.Object,
            _dateTimeExtensionsMock.Object,
            _busMock.Object
        );
    }

    [Fact]
    public async Task RequestTopUpByUserIdAsync_UserDoesNotExist_ReturnsError()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();
        var request = new TopUpRequest(Guid.NewGuid(), Guid.NewGuid()) { BeneficiaryId = Guid.NewGuid(), TopOptionId = Guid.NewGuid() };

        _userWebServiceMock.Setup(x => x.GetFakeUser(It.IsAny<Guid>()))
            .Returns(Result<UserResponse>.Error("User doesn't exist"));

        // Act
        var result = await service.RequestTopUpByUserIdAsync(userId, request);

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be("User doesn't exist");
    }

    [Fact]
    public async Task RequestTopUpByUserIdAsync_CreatePendingTransactionFails_ReturnsError()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();
        var beneficiaryId = Guid.NewGuid();
        var request = new TopUpRequest(Guid.NewGuid(), Guid.NewGuid()) { BeneficiaryId = beneficiaryId, TopOptionId = Guid.NewGuid() };

        _userWebServiceMock.Setup(x => x.GetFakeUser(userId))
            .Returns(Result<UserResponse>.Ok(new UserResponse(userId, true)));

        _beneficiaryRepositoryMock.Setup(x => x.ListTopUpBeneficiariesAsync(It.IsAny<Expression<Func<TopUpBeneficiary, bool>>>()))
            .ReturnsAsync(Result<List<TopUpBeneficiary>>.Ok(new List<TopUpBeneficiary>() { new(userId, "nickname", "+971-04-1234567", true) }));

        _topUpOptionRepositoryMock.Setup(x => x.GetTopUpOptionById(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(Result<TopUpOption>.Error("Top up option not found"));

        // Act
        var result = await service.RequestTopUpByUserIdAsync(userId, request);

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be("Top up option not found");
    }

    [Fact]
    public async Task RequestTopUpByUserIdAsync_WithdrawBalanceFails_ReturnsError()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();
        var request = new TopUpRequest(Guid.NewGuid(), Guid.NewGuid()) { BeneficiaryId = Guid.NewGuid(), TopOptionId = Guid.NewGuid() };

        _userWebServiceMock.Setup(x => x.GetFakeUser(userId))
            .Returns(Result<UserResponse>.Ok(new UserResponse(userId, true)));

        _beneficiaryRepositoryMock.Setup(x => x.ListTopUpBeneficiariesAsync(It.IsAny<Expression<Func<TopUpBeneficiary, bool>>>()))
            .ReturnsAsync(Result<List<TopUpBeneficiary>>.Ok(new List<TopUpBeneficiary>() { new(userId, "nickname", "+971-04-1234567", true) }));

        _topUpOptionRepositoryMock.Setup(x => x.GetTopUpOptionById(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(Result<TopUpOption>.Ok(new TopUpOption("AED", 10, true)));

        _topUpTransactionRepositoryMock.Setup(x =>
        x.ListTopUpTransactionsByUserIdWithinAPeriodAsync(It.IsAny<Guid>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(Result<List<TopUpTransaction>>
                .Ok(new List<TopUpTransaction> { new(userId, It.IsAny<Guid>(), 10, It.IsAny<DateTimeOffset>(), 1) }));

        _topUpTransactionRepositoryMock.Setup(x => x.CreateTopUpTransactionAsync(It.IsAny<TopUpTransaction>()))
            .ReturnsAsync(Result<Guid>.Ok(Guid.NewGuid()));

        _accountWebServiceMock.Setup(x => x.WithdrawBalanceAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<Guid>()))
            .ReturnsAsync(Result<BalanceWithdrawlResponse>.Error("Insufficient funds"));

        // Act
        var result = await service.RequestTopUpByUserIdAsync(userId, request);

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be("Insufficient funds");
    }

    [Fact]
    public async Task RequestTopUpByUserIdAsync_CompletesTopUpTransactionFails_ReturnsError()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();
        var request = new TopUpRequest(Guid.NewGuid(), Guid.NewGuid()) { BeneficiaryId = Guid.NewGuid(), TopOptionId = Guid.NewGuid() };

        _userWebServiceMock.Setup(x => x.GetFakeUser(userId))
            .Returns(Result<UserResponse>.Ok(new UserResponse(userId, true)));

        _beneficiaryRepositoryMock.Setup(x => x.ListTopUpBeneficiariesAsync(It.IsAny<Expression<Func<TopUpBeneficiary, bool>>>()))
            .ReturnsAsync(Result<List<TopUpBeneficiary>>.Ok(new List<TopUpBeneficiary>() { new(userId, "nickname", "+971-04-1234567", true) }));

        _topUpOptionRepositoryMock.Setup(x => x.GetTopUpOptionById(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(Result<TopUpOption>.Ok(new TopUpOption("AED", 10, true)));

        _topUpTransactionRepositoryMock.Setup(x =>
                x.ListTopUpTransactionsByUserIdWithinAPeriodAsync(It.IsAny<Guid>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                    .ReturnsAsync(Result<List<TopUpTransaction>>
                        .Ok(new List<TopUpTransaction> { new(userId, It.IsAny<Guid>(), 10, It.IsAny<DateTimeOffset>(), 1)}));

        _topUpTransactionRepositoryMock.Setup(x => x.CreateTopUpTransactionAsync(It.IsAny<TopUpTransaction>()))
            .ReturnsAsync(Result<Guid>.Ok(Guid.NewGuid()));

        _accountWebServiceMock.Setup(x => x.WithdrawBalanceAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<Guid>()))
            .ReturnsAsync(Result<BalanceWithdrawlResponse>.Ok(new BalanceWithdrawlResponse(BalanceWithdrawlTransactionId: Guid.NewGuid())));

        _topUpTransactionRepositoryMock.Setup(x => x.UpdateTopUpTransactionStatusAsync(It.IsAny<Guid>(), TopUpTransactionStatus.Success, null))
            .ReturnsAsync(Result<Guid>.Error("Error updating transaction status"));

        // Act
        var result = await service.RequestTopUpByUserIdAsync(userId, request);

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be("Error updating transaction status");
    }

    [Fact]
    public async Task RequestTopUpByUserIdAsync_Success_ReturnsTopUpTransactionId()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();
        var request = new TopUpRequest(Guid.NewGuid(), Guid.NewGuid()) { BeneficiaryId = Guid.NewGuid(), TopOptionId = Guid.NewGuid() };

        _userWebServiceMock.Setup(x => x.GetFakeUser(userId))
            .Returns(Result<UserResponse>.Ok(new UserResponse(userId, true)));

        _beneficiaryRepositoryMock.Setup(x => x.ListTopUpBeneficiariesAsync(It.IsAny<Expression<Func<TopUpBeneficiary, bool>>>()))
            .ReturnsAsync(Result<List<TopUpBeneficiary>>.Ok(new List<TopUpBeneficiary>() { new(userId, "nickname", "+971-04-1234567", true) }));

        _topUpOptionRepositoryMock.Setup(x => x.GetTopUpOptionById(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(Result<TopUpOption>.Ok(new TopUpOption("AED", 10, true)));

        _topUpTransactionRepositoryMock.Setup(x => x.CreateTopUpTransactionAsync(It.IsAny<TopUpTransaction>()))
            .ReturnsAsync(Result<Guid>.Ok(Guid.NewGuid()));

        _topUpTransactionRepositoryMock.Setup(x =>
        x.ListTopUpTransactionsByUserIdWithinAPeriodAsync(It.IsAny<Guid>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(Result<List<TopUpTransaction>>
                .Ok(new List<TopUpTransaction> { new(userId, It.IsAny<Guid>(), 10, It.IsAny<DateTimeOffset>(), 1) }));

        _accountWebServiceMock.Setup(x => x.WithdrawBalanceAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<Guid>()))
            .ReturnsAsync(Result<BalanceWithdrawlResponse>.Ok(new BalanceWithdrawlResponse(BalanceWithdrawlTransactionId: Guid.NewGuid())));

        _topUpTransactionRepositoryMock.Setup(x => x.UpdateTopUpTransactionStatusAsync(It.IsAny<Guid>(), TopUpTransactionStatus.Success, null))
            .ReturnsAsync(Result<Guid>.Ok(Guid.NewGuid()));

        // Act
        var result = await service.RequestTopUpByUserIdAsync(userId, request);

        // Assert
        result.HasError.Should().BeFalse();
        result.Data.Should().NotBeEmpty();
    }
}
