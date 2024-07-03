namespace Backend.TopUp.Core.Entities
{
    public class TopUpOption(string currencyAbbreviation, decimal value, bool isActive) 
        : BaseEntity<Guid>(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), null)
    {
        public string CurrencyAbbreviation { get; private set; } = currencyAbbreviation;
        public decimal Value { get; private set; } = value;
        public bool IsActive { get; private set; } = isActive;
    }
}
