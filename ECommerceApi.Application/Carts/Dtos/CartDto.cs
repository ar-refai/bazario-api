using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Carts.Dtos
{
    public sealed record class CartDto
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public IReadOnlyList<CartItemDto> Items { get; init; } = [];
        public decimal Total { get; init; }
        public string Currency { get; init; } = string.Empty;
    }

    public sealed record class CartItemDto
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public decimal UnitPrice { get; init; }
        public string Currency { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal LineTotal { get; init; }
    }

}
