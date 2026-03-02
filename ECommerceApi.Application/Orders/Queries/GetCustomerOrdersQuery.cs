using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Orders.Queries
{
    public sealed record class  GetCustomerOrdersQuery (Guid CustomerId);
}
