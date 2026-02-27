using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Events;
using ECommerceApi.Domain.ValueObjects;

namespace ECommerceApi.Domain.Aggregates.Products
{
    public class Product : AggregateRoot
    {
        private readonly List<ProductVariant> _variants = [];

        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Money Price { get; private set; } = null!;
        public string Category { get; private set; } = string.Empty;
        public int StockQuantity { get; private set; }
        public ProductSku Sku { get; private set; } = null!;

        public bool IsDeleted { get; private set; } 
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

        private Product() { }
        private Product(Guid id, string name, string description, Money price, string category, int stockQuantity, ProductSku sku): base(id)
        {
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            StockQuantity = stockQuantity;
            Sku = sku;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public static Product Create(string name, string description, Money price, string category, int stockQuantity, ProductSku sku)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category is required.", nameof(category));
            if (stockQuantity < 0)
                throw new ArgumentException("Initial stock cannot be negative.", nameof(stockQuantity));

            return new Product(Guid.NewGuid(), name.Trim(), description.Trim(), price, category, stockQuantity, sku);
        }

        public void UpdateDetails(string name, string description, string category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category is required.", nameof(category));
            Name = name;
            Description = description;
            Category = category;
        }

        public void UpdatePrice(Money newPrice)
        {

            Price = newPrice;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity to add must be positive.", nameof(quantity));
            StockQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        internal void ReserveStock(int quantity)
        {
            if(quantity <= 0)
                throw new ArgumentException("Quantity to add must be positive.", nameof(quantity));
            if(StockQuantity < quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for '{Name}'. Available: {StockQuantity}, Requested: {quantity}.");
            StockQuantity -= quantity;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new StockReservedEvent(Id, quantity));
        }

        internal void RestoreStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive.", nameof(quantity));
            StockQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
        }



    }
}
