using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Orders.Commands
{
    public sealed record PlaceOrderCommand(
    Guid CustomerId,
    // Shipping address provided at checkout — might differ from profile address
    string Street,
    string City,
    string Country,
    string PostalCode);
}
