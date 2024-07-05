using Backend.TopUp.Core.Api.Request;
using Backend.TopUp.Core.Application;
using Backend.TopUp.Core.Application.Services;
using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using Backend.TopUp.Core.Extensions;
using Backend.TopUp.Core.Infrastruture.Repositories;
using Backend.TopUp.Core.Infrastruture.WebServices;
using Backend.TopUp.Core.Infrastruture.WebServices.Response;
using Backend.TopUp.Core.Messages;
using MassTransit;

namespace Backend.TopUp.Application.Services
{
    // this class can be huge in some cases...
    // so we can divide it into classes by usecase, for example, or 
    // we can use some approach like CQRS... any others 
    public class TopUpService : ITopUpService
    {
        private readonly ITopUpBeneficiaryRepository _beneficiaryRepository;
        private readonly IUserWebService _userWebService;
        private readonly IBankAccountWebService _accountWebService;
        private readonly IDateTimeExtensions _dateTimeExtensions;
        private readonly ITopUpOptionRepository _topUpOptionRepository;
        private readonly ITopUpTransactionRepository _topUpTransactionRepository;
        private readonly IBus _bus;

        public TopUpService(
            ITopUpBeneficiaryRepository topUpBeneficiaryRepository, 
            ITopUpOptionRepository topUpOptionRepository,
            ITopUpTransactionRepository topUpTransactionRepository,
            IUserWebService userWebService,
            IBankAccountWebService accountWebService,
            IDateTimeExtensions dateTimeExtensions,
            IBus bus)
        {
            _beneficiaryRepository = topUpBeneficiaryRepository;
            _userWebService = userWebService;
            _accountWebService = accountWebService;
            _dateTimeExtensions = dateTimeExtensions;
            _topUpOptionRepository = topUpOptionRepository;
            _topUpTransactionRepository = topUpTransactionRepository;
            _bus = bus;
        }

        public async Task<Result<Guid>> AddTopUpBeneficiaryAsync(Guid userId, AddBeneficiaryRequest request)
        {
            var fakeUserExists = UserExists(userId);
            if (fakeUserExists.HasError) // todo: put some log here
                return Result<Guid>.Error(fakeUserExists.ErrorMessage!);

            // from here to AddTopUpBeneficiaryAsync function we can assume some concurrency,
            // but I'm optimistic about it :)
            var beneficiariesResult = await _beneficiaryRepository.ListTopUpBeneficiariesAsync(x => x.UserId == userId);
            if (beneficiariesResult.HasError) // todo: put some log here
                return Result<Guid>.Error("some error ocurred");
            if (beneficiariesResult.Data!.Count >= TopUpRules.TopUpBeneficiariesLimitPerUser)  // todo: put some log here
                return Result<Guid>.Error($"You have reached the top-up beneficiaries limit: {TopUpRules.TopUpBeneficiariesLimitPerUser}");

            var beneficiaryAlreadyExists = beneficiariesResult.Data.Any(x => x.Nickname == request.Nickname || x.PhoneNumber == request.PhoneNumber);
            if (beneficiaryAlreadyExists) // todo: put some log here
                return Result<Guid>.Error("The top-up beneficiary already exists");

            var topUpBeneficiary = new TopUpBeneficiary(userId, request.Nickname, request.PhoneNumber, true);
            var result = await _beneficiaryRepository.AddTopUpBeneficiaryAsync(topUpBeneficiary);

            if (result.HasError)  // todo: put some log here
                return Result<Guid>.Error("some error ocurred");

            return result;
        }

        public async Task<Result<List<TopUpBeneficiary>>> ListBeneficiariesByUserIdAsync(Guid userId)
        {
            var fakeUserExists = UserExists(userId);
            if (fakeUserExists.HasError) // todo: put some log here
                return Result<List<TopUpBeneficiary>>.Error(fakeUserExists.ErrorMessage!);

            return await _beneficiaryRepository.ListTopUpBeneficiariesAsync(x => x.UserId == userId);
        }

        public async Task<Result<List<TopUpOption>>> ListTopUpOptionsByUserIdAsync(Guid userId, string currencyAbbreviation)
        {
            var fakeUserExists = UserExists(userId);
            if (fakeUserExists.HasError) // todo: put some log here
                return Result<List<TopUpOption>>.Error(fakeUserExists.ErrorMessage!);
            if (currencyAbbreviation == null) // todo: put some log here
                return Result<List<TopUpOption>>.Error("Currency abbreviation value required");

            var result = await _topUpOptionRepository.ListTopUpOptionsAsync(x => x.IsActive && x.CurrencyAbbreviation == currencyAbbreviation);
            if(!result.Data!.Any()) // todo: put some log here
                return Result<List<TopUpOption>>.Error("Currency abbreviation doesn't exist");

            return result;
        }

        public async Task<Result<Guid>> RequestTopUpByUserIdAsync(Guid userId, TopUpRequest request)
        {
            var fakeUserResult = GetUserInfo(userId);
            if (fakeUserResult.HasError) // todo: put some log here
                return Result<Guid>.Error(fakeUserResult.ErrorMessage!);
            var fakeUser = fakeUserResult.Data;

            // from here, may we need some distributed lock to guarantee consistency
            var pendingTransactionResult = await CreatePendingTopUpTransaction(request, fakeUser);
            if (pendingTransactionResult.HasError) // todo: put some log here
                return Result<Guid>.Error(pendingTransactionResult.ErrorMessage!);
            var (topUpTransactionId, topUpValue) = pendingTransactionResult.Data!;

            var balanceWithdrawlResult = await _accountWebService.WithdrawBalanceAsync(fakeUser!.UserId, topUpValue, topUpTransactionId);
            if (balanceWithdrawlResult.HasError) // todo: put some log here
                return Result<Guid>.Error(balanceWithdrawlResult.ErrorMessage!);
            var balanceWithdrawlTransactionId = balanceWithdrawlResult.Data!.BalanceWithdrawlTransactionId;

            return await CompletesTopUpTransaction(userId, topUpTransactionId, topUpValue, balanceWithdrawlTransactionId);
        }

        private async Task<Result<Guid>> CompletesTopUpTransaction(Guid userId, Guid topUpTransactionId, decimal topUpValue, Guid balanceWithdrawlTransactionId)
        {
            var updateTopUpTransactionResult = await _topUpTransactionRepository.UpdateTopUpTransactionStatusAsync(topUpTransactionId, TopUpTransactionStatus.Success);

            if (updateTopUpTransactionResult.HasError)
            {
                // todo: put some log here
                // here we publish an event in a queue to accountWebService to subscribe
                // and set its withdrawl transaction to NotOk and refund money to the user
                await _bus.Publish(new TopUpTransactionFailed(
                    userId,
                    topUpValue,
                    balanceWithdrawlTransactionId,
                    topUpTransactionId));

                return Result<Guid>.Error(updateTopUpTransactionResult.ErrorMessage!);
            }

            return Result<Guid>.Ok(topUpTransactionId);
        }

        private async Task<Result<Tuple<Guid,decimal>>> CreatePendingTopUpTransaction(TopUpRequest request, UserResponse? fakeUser)
        {
            var userId = fakeUser!.UserId;
            var isUserVerified = fakeUser.IsVerified;
            var beneficiaryId = request.BeneficiaryId;
            var topUpOptionId = request.TopOptionId;

            var userAndbeneficiaryRelationship =  await CheckTheRelationshipBetweenUserAndBeneficiary(userId, beneficiaryId);
            if(userAndbeneficiaryRelationship.HasError) // todo: put some log here
                return Result<Tuple<Guid, decimal>>.Error(userAndbeneficiaryRelationship.ErrorMessage!);

            var topUpOptionResult = await _topUpOptionRepository.GetTopUpOptionById(topUpOptionId);
            if (topUpOptionResult.HasError)  // todo: put some log here
                return Result<Tuple<Guid, decimal>>.Error(topUpOptionResult.ErrorMessage!);
            var topUpValue = topUpOptionResult.Data!.Value;

            var transactionsOfTheMonthResult = await GetTopUpTransactionsOfTheMonthByUserId(userId);
            if (transactionsOfTheMonthResult!.HasError)  // todo: put some log here
                return Result<Tuple<Guid, decimal>>.Error(transactionsOfTheMonthResult.ErrorMessage!);
            var topUpTransactionsOfTheMonth = transactionsOfTheMonthResult.Data!;

            var totalAmountForTheMonthForThisBeneficiary = GetTotalTopUpAmountForTheMonthByBeneficiaryId(beneficiaryId, topUpTransactionsOfTheMonth);
            var totalAmountForTheMonth = GetTotalTopUpAmountForTheMonth(topUpTransactionsOfTheMonth);

            if (TopUpRules.HasTheUserReachedTheTopUpLimitThisMonth(totalAmountForTheMonth, topUpValue)) // todo: put some log here
                return Result<Tuple<Guid, decimal>>.Error("User have reached the top-up limit this month.");

            if (TopUpRules.HasTheUserReachedTheTopUpLimitThisMonthForThisBeneficiary(totalAmountForTheMonthForThisBeneficiary, topUpValue, isUserVerified)) // todo: put some log here
                return Result<Tuple<Guid, decimal>>.Error("User have reached the top-up limit for this beneficiary this month.");

            var pendingTransactionObj = TopUpTransaction.NewPendingTransaction(userId, beneficiaryId, topUpValue, TopUpRules.TopUpTransactionCharge);
            var createPendingTransactionResult = await _topUpTransactionRepository.CreateTopUpTransactionAsync(pendingTransactionObj);
            if (createPendingTransactionResult.HasError) // todo: put some log here
                return Result<Tuple<Guid, decimal>>.Error("We have a problem creating the top-up transaction");
            var pendingTopUpTransactionId = createPendingTransactionResult.Data;

            return Result<Tuple<Guid, decimal>>.Ok(Tuple.Create(pendingTopUpTransactionId, topUpValue));
        }

        private async Task<Result<bool>> CheckTheRelationshipBetweenUserAndBeneficiary(Guid userId, Guid beneficiaryId)
        {
            var topUpBeneficiaryListResult = await _beneficiaryRepository
                .ListTopUpBeneficiariesAsync(x => x.UserId == userId && x.Id == beneficiaryId);

            if (topUpBeneficiaryListResult.HasError || topUpBeneficiaryListResult.Data!.Count <= 0) // todo: put some log here
                return Result<bool>.Error("It was not possible retrieve this beneficiary or user doesn't have relationship with this beneficiary.");

            return Result<bool>.Ok(true);
        }

        private static decimal GetTotalTopUpAmountForTheMonth(List<TopUpTransaction> transactionsOfTheMontht)
        {
            return transactionsOfTheMontht
                .Where(x => x.Status == TopUpTransactionStatus.Success)
                .Sum(x => x.Amount);
        }

        private static decimal GetTotalTopUpAmountForTheMonthByBeneficiaryId(Guid beneficiaryId, List<TopUpTransaction> topUpTransactionsOfTheMonth)
        {
            return topUpTransactionsOfTheMonth
                .Where(x => x.TopUpBeneficiaryId == beneficiaryId && x.Status == TopUpTransactionStatus.Success)
                .Sum(x => x.Amount);
        }

        private async Task<Result<List<TopUpTransaction>>?> GetTopUpTransactionsOfTheMonthByUserId(Guid userId)
        {
            var now = _dateTimeExtensions.NowUtc();
            var firstDateOfTheMonth = _dateTimeExtensions.NewDateUtc(new DateTime(now.Year, now.Month, 1));

            return await _topUpTransactionRepository
                .ListTopUpTransactionsByUserIdWithinAPeriodAsync(userId, firstDateOfTheMonth, now);
        }

        // this validation can be called into some middleware
        // after reading/validating info based on a token 
        private Result<bool> UserExists(Guid userId)
        {
            var fakeUserExistsResult = _userWebService.FakeUserExists(userId);

            if (fakeUserExistsResult.HasError) // todo: put some log here
                return Result<bool>.Error("Some error happened calling user service");

            if (!fakeUserExistsResult.Data) // todo: put some log here
                return Result<bool>.Error("User doesn't exists");

            return Result<bool>.Ok(true);
        }
        private Result<UserResponse> GetUserInfo(Guid userId)
        {
            var fakeUserResult = _userWebService.GetFakeUser(userId);

            if (fakeUserResult.HasError) // todo: put some log here
                return Result<UserResponse>.Error(fakeUserResult.ErrorMessage!);

            return Result<UserResponse>.Ok(fakeUserResult.Data!);
        }
    }
}
