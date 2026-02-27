using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;

namespace ECommerceApi.Domain.ValueObjects
{
    public sealed class ProductSku : ValueObject
    {
        private static readonly Regex SkuRegex = new(
        @"^[A-Z0-9][A-Z0-9\-]{2,18}[A-Z0-9]$",
        RegexOptions.Compiled);
        public string Value { get; }
        private ProductSku()
        {
            Value = string.Empty;
        }
        public ProductSku (string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Product SKU cannot be empty.");
            var normalized = value.Trim().ToLowerInvariant();
            if(!SkuRegex.IsMatch(normalized))
                throw new ArgumentException(
                $"'{value}' is not a valid SKU. Format: uppercase letters/numbers/hyphens, 4–20 chars.",
                nameof(value));

            Value = value;
        }

        protected override IEnumerable<object?> getEqualityComponents()
        {
            yield return Value;
        }
        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
