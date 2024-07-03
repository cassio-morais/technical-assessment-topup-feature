namespace Backend.TopUp.Core.Api.Response
{
    public sealed record TopUpOptionResponse
    {
        public TopUpOptionResponse(Guid id, string currencyAbbreviation, decimal value)
        {
            Id = id;
            CurrencyAbbreviation = currencyAbbreviation;
            Value = value;
        }

        public Guid Id { get; set; }
        public string CurrencyAbbreviation { get; private set; }
        public decimal Value { get; private set; }
    }
}
