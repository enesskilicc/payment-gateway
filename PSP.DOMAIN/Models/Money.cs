using PSP.DOMAIN.Exceptions;

namespace PSP.DOMAIN.Models
{
    public sealed record Money
    {
        public long Amount { get; }
        public string Currency { get; }

        public Money(long amount, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new DomainValidationException(DomainErrorCodes.InvalidCurrency, "Para birimi belirtilmemiş!");

            if (currency.Length != 3 || !currency.All(char.IsAsciiLetterUpper))
                throw new DomainValidationException(DomainErrorCodes.InvalidCurrency, "Para birimi geçersiz!");

            Amount = amount;
            Currency = currency;
        }


        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainValidationException(DomainErrorCodes.CurrencyMismatch, "Para birimleri eşleşmiyor!");

            return new Money(checked(Amount + other.Amount), Currency);
        }
        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainValidationException(DomainErrorCodes.CurrencyMismatch, "Para birimleri eşleşmiyor!");

            return new Money(checked(Amount - other.Amount), Currency);
        }

        public bool IsGreaterThan(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainValidationException(DomainErrorCodes.CurrencyMismatch, "Para birimleri eşleşmiyor!");

            return Amount > other.Amount;
        }
    }
}
