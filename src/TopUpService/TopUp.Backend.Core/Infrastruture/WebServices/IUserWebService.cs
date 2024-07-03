using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Infrastruture.WebServices.Response;

namespace Backend.TopUp.Core.Infrastruture.WebServices
{
    public interface IUserWebService
    {
        Result<UserResponse> GetFakeUser(Guid userId);
        Result<bool> FakeUserExists(Guid userId);
    }
}
