using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.ValueObjects;

namespace ECommerceApi.Domain.Aggregates.Carts
{
    public class ShoppingCart : AggregateRoot
    {
        private readonly List<CartItem> _items = [];

        public Guid CustomerId { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();
        public Money Total
        {
            get
            {
                if (_items.Count == 0) return Money.Zero(Enums.Currency.USD);
                var currency = _items.First().UnitPrice.Currency;
                return _items.Aggregate(Money.Zero(currency), (sum,item) => sum.Add(item.LineTotal));
            }
        }

        private ShoppingCart(Guid id, Guid customerId) : base(id)
        {
            CustomerId = customerId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public static ShoppingCart Create(Guid customerId)
        {
            return new ShoppingCart(Guid.NewGuid(), customerId);
        }

        public void AddItem(Guid productId, string productName, Money unitPrice, int quantity)
        {
            var existing = _items.FirstOrDefault(i => i.ProductId == productId);
            if (existing is not null)
            {
                existing.UpdateQuantity(existing.Quantity + quantity);
            }
            else
            {
                _items.Add(new CartItem(Guid.NewGuid(), productId, productName, unitPrice, quantity));
            }
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void UpdateItemQuantity(Guid productId, int newQuantity)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId) ?? throw new InvalidOperationException($"Product {productId} is not in the cart.");
            if(newQuantity <= 0)
            {
                _items.Remove(item);
            }else
            {
                item.UpdateQuantity(newQuantity);
            }
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveItem(Guid productId)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId) ?? throw new InvalidOperationException($"Product {productId} is not in the cart.");
            _items.Remove(item);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ClearCart()
        {
            _items.Clear();
            UpdatedAt = DateTime.UtcNow;
        }

        public bool BelongsTo(Guid customerId) => CustomerId == customerId;
    }
}
