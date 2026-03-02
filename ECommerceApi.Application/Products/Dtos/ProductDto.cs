using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Products.Dtos
{
    public sealed record ProductDto
    {
        // id, name, description, price, currency, category, stockquantity, sku, isdeleted, createdat, updattedat.
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty; 
        public decimal Price { get; init; }
        public string Currency { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public int StockQuantity { get; init; }
        public string Sku { get; init; } = string.Empty;
        public bool IsDelted { get; init; }

        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}
