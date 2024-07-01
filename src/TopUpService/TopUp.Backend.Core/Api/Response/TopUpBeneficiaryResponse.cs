namespace Backend.TopUp.Core.Api.Response
{
    public class TopUpBeneficiaryResponse
    {
        public TopUpBeneficiaryResponse(Guid id, string nickname, string phoneNumber, bool isActive)
        {
            Id = id;
            Nickname = nickname;
            PhoneNumber = phoneNumber;
            IsActive = isActive;
        }

        public Guid Id { get; private set; }
        public string Nickname { get; private set; }
        public string PhoneNumber { get; private set; }
        public bool IsActive { get; private set; }
    }
}
