using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Events
{
    public sealed record OrderPlacedEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid OrderId,
    Guid CustomerId,
    string OrderNumber) : IDomainEvent
    {
        public OrderPlacedEvent(Guid orderId, Guid customerId, string orderNumber) : this(Guid.NewGuid(), DateTime.UtcNow, orderId, customerId, orderNumber) { }
    }
}
