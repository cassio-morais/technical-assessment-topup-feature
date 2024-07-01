namespace Backend.TopUp.Core.Infrastruture.WebServices
{
    public interface IUserWebService
    {
        bool IsVerified(Guid userId);
        bool Exists(Guid userId);
    }
}
