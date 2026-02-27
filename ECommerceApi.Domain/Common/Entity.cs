using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Common
{
    /// <summary>
    /// Represents a base class for entities with a unique identifier and value-based equality.
    /// </summary>
    public abstract class Entity
    {
        public Guid Id { get; private set; }

        protected Entity() { }
        protected Entity(Guid id)
        {
            if(id == Guid.Empty) throw new ArgumentNullException("Entity id can not be empty",nameof(id));
            Id = id;
        }
        public override bool Equals(object? obj)
        {
            if (obj is not Entity other) return false;
            if(ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            return Id == other.Id;
        }
        public override int GetHashCode() => Id.GetHashCode();
        public static bool operator ==(Entity? left, Entity? right) => left?.Equals(right) ?? right is null;
        public static bool operator !=(Entity? left, Entity? right) => !(left == right);
    }
}
