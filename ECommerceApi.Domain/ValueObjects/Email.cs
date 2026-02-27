using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;
namespace ECommerceApi.Domain.ValueObjects
{
    public sealed class Email : ValueObject
    {
        private static readonly Regex EmailRegex = new (@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public string Value { get; }
        private Email () { Value = string.Empty; }
        public Email(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Email cannot be empty.");
            var normalized = value.Trim().ToLowerInvariant();

            if (!Email.EmailRegex.IsMatch(normalized))
                throw new ArgumentException($"The provided email : {value} is not valid.", nameof(value));
            Value = normalized;
        }

        protected override IEnumerable<object?> getEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
