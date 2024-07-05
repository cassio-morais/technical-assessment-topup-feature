using System;

namespace Backend.TopUp.Core.Entities
{
    public class TopUpOption : BaseEntity<Guid>
    {
        public TopUpOption(string currencyAbbreviation, decimal value, bool isActive)
            : base(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), null)
        {

            CurrencyAbbreviation = currencyAbbreviation;
            Value = value;
            IsActive = isActive;
        }

        private string _currencyAbbreviation;
        public string CurrencyAbbreviation
        {
            get
            {
                return _currencyAbbreviation;
            }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Currency abbreviation cannot be null or empty.");

                _currencyAbbreviation = value;
            }
        }
        private decimal _value;
        public decimal Value 
        {
            get
            {
                return _value;
            }
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");

                _value = value;
            }
        }

        public bool IsActive { get; private set; }
    }
}
