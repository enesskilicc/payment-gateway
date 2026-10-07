using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.DOMAIN.Models
{
    public sealed record Money
    {
        public long Amount { get; }
        public string Currency { get; }

        public Money(long amount, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Para birimi belirtilmemiş!", nameof(currency));

            if (currency.Length != 3 || !currency.All(char.IsAsciiLetterUpper))
                throw new ArgumentException("Para birimi geçersiz!", nameof(currency));

            Amount = amount;
            Currency = currency;
        }


        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new ArgumentException("Para birimleri eşleşmiyor!", nameof(other));

            return new Money(checked(Amount + other.Amount), Currency);
        }
    }
}
