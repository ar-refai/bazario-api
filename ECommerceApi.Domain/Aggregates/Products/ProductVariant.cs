using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.ValueObjects;

namespace ECommerceApi.Domain.Aggregates.Products
{
    /// <summary>
    /// Child Entity that represents a specific version of a product (like a "Red, Large T-Shirt" vs. a "Blue, Small T-Shirt").
    /// </summary>
    public sealed class ProductVariant : Entity
    {
        public Guid ProductId { get; private set; }
        public ProductSku Sku { get; private set; } = null!;
        public string Attributes { get; private set; } = string.Empty;
        public int StockQuantity { get; private set; }
        public Money PriceModifier { get; private set; } = null!;

        private ProductVariant() { }
        internal ProductVariant(Guid productId, ProductSku sku, string attributes, int stockQuantity,  Money priceModifier) : base(Guid.NewGuid())
        {
            ProductId = productId;
            Sku = sku;
            Attributes = attributes;
            StockQuantity = stockQuantity;
            PriceModifier = priceModifier;
        }
    }
}
