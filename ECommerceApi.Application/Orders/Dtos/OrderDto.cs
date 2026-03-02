using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Orders.Dtos
{
    public sealed record OrderDto
    {
        public Guid Id { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public decimal TotalAmount { get; init; }
        public string Currency { get; init; } = string.Empty;
        public AddressDto ShippingAddress { get; init; } = null!;
        public IReadOnlyList<OrderItemDto> Items { get; init; } = [];
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }

    public sealed record AddressDto
    {
        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;

    }

    public sealed record OrderItemDto
    {
        public Guid Id { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public decimal UnitPrice { get; init; }
        public string Currency { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal LineTotal { get; init; }
    }
}
