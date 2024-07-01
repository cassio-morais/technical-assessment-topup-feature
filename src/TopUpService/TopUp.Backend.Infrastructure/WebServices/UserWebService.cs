using Backend.FakeUser;
using Backend.TopUp.Core.Infrastruture.WebServices;

namespace Backend.TopUp.Infrastructure.WebServices
{
    public class UserWebService : IUserWebService
    {
        public bool Exists(Guid userId)
        {
            return FakeUserService.Users.Exists(x => x.UserId == userId);
        }

        public bool IsVerified(Guid userId)
        {
            return FakeUserService.Users.Exists(x => x.UserId == userId && x.IsVerified);
        }
    } 
}
