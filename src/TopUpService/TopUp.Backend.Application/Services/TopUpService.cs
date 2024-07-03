using Backend.TopUp.Core.Api.Request;
using Backend.TopUp.Core.Application;
using Backend.TopUp.Core.Application.Services;
using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using Backend.TopUp.Core.Extensions;
using Backend.TopUp.Core.Infrastruture.Repositories;
using Backend.TopUp.Core.Infrastruture.WebServices;
using Backend.TopUp.Core.Infrastruture.WebServices.Response;

namespace Backend.TopUp.Application.Services
{
    // this class can be split into classes by usecase, for example, or 
    // we can use some approach like CQS... any others 
    public class TopUpService(
        ITopUpBeneficiaryRepository topUpBeneficiaryRepository, 
        ITopUpOptionRepository topUpOptionRepository,
        ITopUpTransactionRepository topUpTransactionRepository,
        IUserWebService userWebService,
        IBankAccountWebService accountWebService,
        IDateTimeExtensions dateTimeExtensions) : ITopUpService
    {
        private readonly ITopUpBeneficiaryRepository _beneficiaryRepository = topUpBeneficiaryRepository;
        private readonly IUserWebService _userWebService = userWebService;
        private readonly IBankAccountWebService _accountWebService = accountWebService;
        private readonly IDateTimeExtensions _dateTimeExtensions = dateTimeExtensions;
        private readonly ITopUpOptionRepository _topUpOptionRepository = topUpOptionRepository;
        private readonly ITopUpTransactionRepository _topUpTransactionRepository = topUpTransactionRepository;

        public async Task<Result<Guid>> AddTopUpBeneficiaryAsync(Guid userId, AddBeneficiaryRequest request)
        {
            var fakeUserExists = UserExists(userId);

            if (fakeUserExists.HasError)
                return Result<Guid>.Error(fakeUserExists.ErrorMessage!);

            // from here to AddTopUpBeneficiaryAsync function we can assume some concurrency,
            // but I'm optimistic about it :)
            var beneficiariesResult = await _beneficiaryRepository.ListTopUpBeneficiariesAsync(x => x.UserId == userId);

            if (beneficiariesResult.HasError) // todo: LOG
                return Result<Guid>.Error("some error ocurred");

            if (beneficiariesResult.Data!.Count >= TopUpRules.TopUpBeneficiariesLimitPerUser)  // todo: LOG
                return Result<Guid>.Error($"You have reached the top-up beneficiaries limit: {TopUpRules.TopUpBeneficiariesLimitPerUser}");

            var beneficiaryAlreadyExists = beneficiariesResult.Data.Any(x => x.Nickname == request.Nickname || x.PhoneNumber == request.PhoneNumber);
            if (beneficiaryAlreadyExists)
                return Result<Guid>.Error("The top-up beneficiary already exists");

            var topUpBeneficiary = new TopUpBeneficiary(userId, request.Nickname, request.PhoneNumber, true);

            var result = await _beneficiaryRepository.AddTopUpBeneficiaryAsync(topUpBeneficiary);

            if (result.HasError)  // todo: LOG
                return Result<Guid>.Error("some error ocurred");

            return result;
        }

        public async Task<Result<List<TopUpBeneficiary>>> ListBeneficiariesByUserIdAsync(Guid userId)
        {
            var fakeUserExists = UserExists(userId);

            if (fakeUserExists.HasError)
                return Result<List<TopUpBeneficiary>>.Error(fakeUserExists.ErrorMessage!);

            return await _beneficiaryRepository.ListTopUpBeneficiariesAsync(x => x.UserId == userId);
        }

        public async Task<Result<List<TopUpOption>>> ListTopUpOptionsByUserIdAsync(Guid userId, string currencyAbbreviation)
        {
            var fakeUserExists = UserExists(userId);

            if (fakeUserExists.HasError) // todo: log
                return Result<List<TopUpOption>>.Error(fakeUserExists.ErrorMessage!);

            if (currencyAbbreviation == null) // todo: LOG
                return Result<List<TopUpOption>>.Error("Currency abbreviation value required");

            var result = await _topUpOptionRepository.ListTopUpOptionsAsync(x => x.IsActive && x.CurrencyAbbreviation == currencyAbbreviation);

            if(!result.Data!.Any()) // todo: LOG
                return Result<List<TopUpOption>>.Error("Currency abbreviation doesn't exist");

            return result;
        }

        public async Task<Result<Guid>> RequestTopUpByUserId(Guid userId, TopUpRequest request)
        {
            var fakeUserResult = GetUserInfo(userId);
            if (fakeUserResult.HasError)
                return Result<Guid>.Error(fakeUserResult.ErrorMessage!);
            
            var fakeUser = fakeUserResult.Data;

            // from here, may we need some distributed lock to guarantee consistency
            var pendingTransactionResult = await CreatePendingTopUpTransaction(request, fakeUser);

            if (pendingTransactionResult.HasError)
                return Result<Guid>.Error(pendingTransactionResult.ErrorMessage!);

            var (transactionId, topUpValue) = pendingTransactionResult.Data!;
            var withdrawResult = await _accountWebService.WithdrawFromBalance(fakeUser!.UserId, topUpValue, transactionId);

            if (withdrawResult.HasError)
            {
                // todo: here we can publish an event in a queue to accountWebService to refund money and to this service to set status transaction to error
                return Result<Guid>.Error(withdrawResult.ErrorMessage!);
            }

            // todo: here we can publish an event in a queue to accountWebService set your transaction to Ok and to this service to set transaction status to success
            return Result<Guid>.Ok(transactionId);
        }

        private async Task<Result<Tuple<Guid,decimal>>> CreatePendingTopUpTransaction(TopUpRequest request, UserResponse? fakeUser)
        {
            var topUpBeneficiaryResult = await _beneficiaryRepository.GetTopUpBeneficiaryById(request.BeneficiaryId);

            if (topUpBeneficiaryResult.HasError)  // todo: LOG
                return Result<Tuple<Guid, decimal>>.Error(topUpBeneficiaryResult.ErrorMessage!);

            var topUpOptionResult = await _topUpOptionRepository.GetTopUpOptionById(request.TopOptionId);

            if (topUpOptionResult.HasError)  // todo: LOG
                return Result<Tuple<Guid, decimal>>.Error(topUpOptionResult.ErrorMessage!);

            var topUpValue = topUpOptionResult.Data!.Value;

            var transactionsOfTheMonthResult = await GetTopUpTransactionsOfTheMonth(fakeUser!);

            if (transactionsOfTheMonthResult!.HasError)  // todo: LOG
                return Result<Tuple<Guid, decimal>>.Error(transactionsOfTheMonthResult.ErrorMessage!);

            var totalAmountForTheMonthPerBeneficiary = GetTotalAmountForTheMonthForThisBeneficiary(request.BeneficiaryId, transactionsOfTheMonthResult);
            var totalAmountForTheMonth = GetTotalAmountForTheMonth(transactionsOfTheMonthResult);

            if (TopUpRules.HasTheUserReachedTheTopUpLimitThisMonth(totalAmountForTheMonth!.Value, topUpValue))
                return Result<Tuple<Guid, decimal>>.Error("User have reached the top-up limit this month.");

            if (TopUpRules.HasTheUserReachedTheTopUpLimitThisMonthForThisBeneficiary(totalAmountForTheMonthPerBeneficiary!.Value, topUpValue, fakeUser.IsVerified))
                return Result<Tuple<Guid, decimal>>.Error("User have reached the top-up limit for this beneficiary this month.");

            var pendingTransaction = TopUpTransaction.NewPendingTransaction(fakeUser.UserId,
                topUpBeneficiaryResult.Data!.Id,
                topUpOptionResult.Data.Value);

            var createTransactionResult = await _topUpTransactionRepository.CreateTopUpTransaction(pendingTransaction);

            if (createTransactionResult.HasError)
                return Result<Tuple<Guid, decimal>>.Error("We have a problem creating the top-up transaction");

            return Result<Tuple<Guid, decimal>>.Ok(Tuple.Create(createTransactionResult.Data, topUpValue));
        }

        private static decimal? GetTotalAmountForTheMonth(Result<List<TopUpTransaction>>? transactionsOfTheMonthResult)
        {
            return transactionsOfTheMonthResult?.Data?
                .Where(x => x.Status == TopUpTransactionStatus.Success)
                .Sum(x => x.Amount);
        }

        private static decimal? GetTotalAmountForTheMonthForThisBeneficiary(Guid beneficiaryId, Result<List<TopUpTransaction>>? transactionsOfTheMonthResult)
        {
            return transactionsOfTheMonthResult?.Data?
                .Where(x => x.TopUpBeneficiaryId == beneficiaryId && x.Status == TopUpTransactionStatus.Success)
                .Sum(x => x.Amount);
        }

        private async Task<Result<List<TopUpTransaction>>?> GetTopUpTransactionsOfTheMonth(UserResponse fakeUser)
        {
            var now = _dateTimeExtensions.NowUtc();
            var firstDateOfTheMonth = _dateTimeExtensions.NewDateUtc(new DateTime(now.Year, now.Month, 1));

            return await _topUpTransactionRepository
                .ListTopUpTransactionsByUserIdWithinAPeriod(fakeUser!.UserId, firstDateOfTheMonth, now);
        }

        // this validation can be called into some middleware
        // after reading/validating info based on a token 
        private Result<bool> UserExists(Guid userId)
        {
            var fakeUserExistsResult = _userWebService.FakeUserExists(userId);

            if (fakeUserExistsResult.HasError) // todo: LOG
                return Result<bool>.Error("Some error happened calling user service");

            if (!fakeUserExistsResult.Data) // todo: LOG
                return Result<bool>.Error("User doesn't exists");

            return Result<bool>.Ok(true);
        }
        private Result<UserResponse> GetUserInfo(Guid userId)
        {
            var fakeUserResult = _userWebService.GetFakeUser(userId);

            if (fakeUserResult.HasError) // todo: LOG
                return Result<UserResponse>.Error(fakeUserResult.ErrorMessage!);

            return Result<UserResponse>.Ok(fakeUserResult.Data!);
        }
    }
}
