using Backend.TopUp.Core.Contracts;

namespace Backend.TopUp.Core.Application
{
    // these rules could be in a database table like "top_up_rules"
    // or something so that can be changed by a backoffice system
    public static class TopUpRules
    {
        public const int TopUpBeneficiariesLimitPerUser = 5;
        private const decimal MonthlyTopUpLimitPerUnverifiedUserPerBeneficiary = 500;
        private const decimal MonthlyTopUpLimitPerVerifiedUserPerBeneficiary = 1000;
        private const decimal MonthlyTopUpLimit = 3000;
        private const decimal TopUpTransactionCharge = 1;

        public static bool HasTheUserReachedTheTopUpLimitThisMonthForThisBeneficiary(decimal totalAmountForTheMonthPerBeneficiary, decimal topUpValue, bool isUserVerified) 
        {
            var transactionTotalCost = topUpValue + TopUpTransactionCharge;

            if (isUserVerified)
            {
                //If a user is verified, they can top up a maximum of AED 1, 000 per calendar month per
                //beneficiary.
                if (totalAmountForTheMonthPerBeneficiary + transactionTotalCost > MonthlyTopUpLimitPerVerifiedUserPerBeneficiary)
                    return true;
            }
            else
            {
                //If a user is not verified, they can top up a maximum of AED 500 per calendar month per
                //beneficiary for security reasons.
                if (totalAmountForTheMonthPerBeneficiary + transactionTotalCost > MonthlyTopUpLimitPerUnverifiedUserPerBeneficiary)
                    return true;
                    //return Result<Tuple<Guid, decimal>>.Error("User have reached the top-up limit for this beneficiary this month");
            }

            return false;
        }

        public static bool HasTheUserReachedTheTopUpLimitThisMonth(decimal totalAmountForTheMonth, decimal topUpValue) 
        {
            var transactionTotalCost = topUpValue + TopUpTransactionCharge;

            if (totalAmountForTheMonth + transactionTotalCost > MonthlyTopUpLimit)
                return true;

            return false;
        }
    }
}
