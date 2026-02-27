using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;

namespace ECommerceApi.Domain.ValueObjects
{
    public sealed class OrderNumber : ValueObject
    {
        public string Value { get; }
        private OrderNumber() { Value = string.Empty; }
        private OrderNumber(string value) => Value = value;

        public static OrderNumber Generate()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Random.Shared.Next(1000, 9999);
            return new OrderNumber($"ORD-{datePart}-{randomPart}");
        }

        /// <summary>
        /// reconstructs new OrderNumber from a string used by efCore
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns>OrderNumber</returns>
        public static OrderNumber From(string orderNumber)
        {
            if(string.IsNullOrEmpty(orderNumber))
                throw new ArgumentException("Order number cannot be empty.", nameof(orderNumber));
            return new OrderNumber(orderNumber);
        }

        protected override IEnumerable<object?> getEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;


    }
}
