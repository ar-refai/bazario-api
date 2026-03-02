using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Orders.Queries
{
    public sealed record class GetOrderQuery(Guid OrderId,Guid RequestingCustomerId, bool IsAdmin);
}
