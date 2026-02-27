using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Events
{
    public sealed record StockReservedEvent (Guid EventId, DateTime OccurredOn, Guid ProductId, int QuantityReserved) : IDomainEvent
    {
        public StockReservedEvent(Guid productId, int quantityReserved) : this(Guid.NewGuid(), DateTime.UtcNow, productId, quantityReserved) { }
    }
}
