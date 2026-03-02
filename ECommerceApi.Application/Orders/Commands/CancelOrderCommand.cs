using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Orders.Commands
{
    public sealed record CancelOrderCommand(Guid OrderId, Guid RequestingCustomerId, bool IsAdmin);
}
