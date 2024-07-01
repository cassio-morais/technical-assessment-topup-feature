using Backend.TopUp.Core.Api.Request;
using Backend.TopUp.Core.Application.Services;
using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Entities;
using Backend.TopUp.Core.Infrastruture.Repository;
using Backend.TopUp.Core.Infrastruture.WebServices;

namespace Backend.TopUp.Application.Services
{
    public class TopUpService(ITopUpRepository context, IUserWebService userWebService) : ITopUpService
    {
        // this rule (and others) could be in a database table like "top_up_rules"
        // or something so that could be changed by a backoffice system
        private const int _topUpBeneficiariesLimit = 5;

        private readonly ITopUpRepository _repository = context;
        private readonly IUserWebService _userWebService = userWebService;

        public async Task<Result<Guid>> AddTopUpBeneficiaryAsync(Guid userId, AddBeneficiaryRequest request)
        {
            // this validation could be calling inside a some middleware after reading/validating info based on a token 
            var fakeUserExists = _userWebService.Exists(userId);

            if (!fakeUserExists)
                return Result<Guid>.Error("User doesn't exist");

            // we can assume some concurrency here, but I'm optimistic by that :)
            var beneficiariesResult = await _repository.ListBeneficiariesAsync(x => x.UserId == userId);

            if (beneficiariesResult.HasError) // todo: LOG
                return Result<Guid>.Error("some error ocurred");

            if (beneficiariesResult.Data!.Count >= _topUpBeneficiariesLimit)  // todo: LOG
                return Result<Guid>.Error($"You have reached the top-up beneficiaries limit: {_topUpBeneficiariesLimit}");

            var beneficiaryAlreadyExists = beneficiariesResult.Data.Any(x => x.Nickname == request.Nickname || x.PhoneNumber == request.PhoneNumber); 
            if (beneficiaryAlreadyExists)
                return Result<Guid>.Error("The top-up beneficiary already exists");

            var topUpBeneficiary = new TopUpBeneficiary(userId, request.Nickname, request.PhoneNumber, true);

            var result = await _repository.AddTopUpBeneficiaryAsync(topUpBeneficiary);

            if (result.HasError)  // todo: LOG
                return Result<Guid>.Error("some error ocurred");

            return result;
        }

        public async Task<Result<List<TopUpBeneficiary>>> ListBeneficiariesByUserId(Guid userId)
        {
            // this validation could be calling inside a some middleware after reading/validating info based on a token 
            var fakeUserExists = _userWebService.Exists(userId);

            if (!fakeUserExists)
                return Result<List<TopUpBeneficiary>>.Error("User doesn't exist");

            return await _repository.ListBeneficiariesAsync(x => x.UserId == userId);

        }
    }
}
