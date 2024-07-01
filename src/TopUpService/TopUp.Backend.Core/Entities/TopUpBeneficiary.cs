namespace Backend.TopUp.Core.Entities
{
    public sealed class TopUpBeneficiary : BaseEntity<Guid>
    {
        public TopUpBeneficiary(Guid userId, string? nickname, string? phoneNumber, bool isActive) 
            : base(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), null)
        {
            UserId = userId;
            PhoneNumber = phoneNumber;
            IsActive = isActive;
            Nickname = nickname;
        }

        public Guid UserId { get; private set; }

        private string? _nickname { get; set; }

        public string? Nickname
        {
            get
            {
                return _nickname;
            }
            private set
            {
                if (value is null || value.Length > 20)
                    throw new Exception("nickname can´t not be null or greater than 20 characters");

                _nickname = value;
            }
        }
        public string? PhoneNumber { get; private set; }
        public bool IsActive { get; private set; }
    }
}
