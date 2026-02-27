using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Enums;

namespace ECommerceApi.Domain.ValueObjects
{
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public Currency Currency { get; }

        private Money () { }
        public Money (decimal amount, Currency currency)
        {
            if (amount < 0) throw new ArgumentException("Monet cannot be negative", nameof(amount));
            Amount = Math.Round(amount,2);
            Currency = currency;
        }

        public static Money Zero(Currency cur) => new Money(0, cur);

        public Money Add(Money other)
        {
            if (Currency != other.Currency) 
                throw new ArgumentException("Cannot add money with different currencies", nameof(other.Currency));
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Multiply(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
            return new Money(Amount * quantity, Currency);
        }

        protected override IEnumerable<object?> getEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public static bool operator >(Money lhs, Money rhs)
        {
            EnsureSameCurrency(lhs, rhs);
            return lhs.Amount > rhs.Amount;
        }

        public static bool operator <(Money lhs, Money rhs)
        {
            EnsureSameCurrency(lhs, rhs);
            return lhs.Amount < rhs.Amount;
        }


        public static bool operator<=(Money lhs, Money rhs)
        {
            EnsureSameCurrency(lhs,rhs);
            return lhs.Amount <= rhs.Amount;
        }

        public static bool operator>=(Money lhs, Money rhs)
        {
            EnsureSameCurrency(lhs, rhs);
            return lhs.Amount >= rhs.Amount;
        }
        
        public static void EnsureSameCurrency(Money lhs, Money rhs)
        {
            if (lhs.Currency != rhs.Currency)
                throw new InvalidOperationException("Cannot compare money with different currencies.");
        }

        public override string ToString() => $"{Amount:F2} {Currency}"; 

    }
}
