using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Common
{
    /// <summary>
    /// Base class for value objects that defines equality based on the values of their components.
    /// </summary>
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected abstract IEnumerable<object?> getEqualityComponents();

        public bool Equals(ValueObject? other)
        {
            if (other is null) return false;
            if (GetType() != other.GetType()) return false;
            return getEqualityComponents().SequenceEqual(other.getEqualityComponents());
        }
        public override bool Equals(object? obj) => obj is ValueObject o && Equals(o);
        public override int GetHashCode() => getEqualityComponents().Aggregate(0, (hash, component) => HashCode.Combine(hash, component?.GetHashCode() ?? 0));
        public static bool operator ==(ValueObject? left, ValueObject? right) => left?.Equals(right) ?? right is null;
        public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
    }
}
