using Backend.FakeUser;
using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Infrastruture.WebServices;
using Backend.TopUp.Core.Infrastruture.WebServices.Response;

namespace Backend.TopUp.Infrastructure.WebServices
{
    public class UserWebService : IUserWebService
    {
        public Result<bool> FakeUserExists(Guid userId)
        {
            return Result<bool>.Ok(FakeUserService.Users.Exists(x => x.UserId == userId));
        }

        public Result<UserResponse> GetFakeUser(Guid userId)
        {
            var user = FakeUserService.Users.FirstOrDefault(x => x.UserId == userId);

            if (user == null)
                return Result<UserResponse>.Error("User doesn't exist");

            return Result<UserResponse>.Ok(new UserResponse(user.UserId, user.IsVerified));
        }
    }
}
