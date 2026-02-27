using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Enums;
using ECommerceApi.Domain.Events;
using ECommerceApi.Domain.ValueObjects;

namespace ECommerceApi.Domain.Aggregates.Orders
{
    public sealed class Order : AggregateRoot
    {
        private readonly List<OrderItem> _items = [];

        public Guid CustomerId { get; private set; }
        public OrderNumber OrderNumber { get; private set; } = null!;
        public OrderStatus Status { get; private set; }
        public Address ShippingAddress { get; private set; } = null!;

        public Money TotalAmount { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order () { }
        private Order (Guid id, Guid customerId, Address shippingAddress, OrderNumber orderNumber) : base(id) 
        {
            CustomerId = customerId;
            ShippingAddress = shippingAddress;
            OrderNumber = orderNumber;
            Status = OrderStatus.Pending;
            TotalAmount = Money.Zero(Currency.USD); // Recalculated when items are added
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public static Order Create(Guid customerId,Address shippingAddress)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer ID is required.", nameof(customerId));
            return new Order(Guid.NewGuid(), customerId, shippingAddress, OrderNumber.Generate());
        }
   
        public void AddItem(Guid productId, string productName, Money unitPrice, int quantity)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Items can only be added to a pending order.");

            // avoid duplicates 
            if (_items.Any(i => i.ProductId == productId))
                throw new InvalidOperationException($"Product {productName} is already in this order.");
            _items.Add(new OrderItem(Id, productId, productName, unitPrice,quantity));
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Transitioning Pending -> Processing.
        /// Called after all items are added and stock has been reserved.
        /// </summary>
        public void Place()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException();
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException();
            Status = OrderStatus.Processing;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new OrderPlacedEvent(Id,CustomerId,OrderNumber.Value));
        }

        public void MarkAsShipped()
        {
            if (Status != OrderStatus.Processing)
                throw new InvalidOperationException($"Cannot ship an order with status '{Status}'. Expected: Processing.");
            Status = OrderStatus.Shipped;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void MarkAsDelivered()
        {
            if (Status != OrderStatus.Shipped)
                throw new InvalidOperationException("Order must be shipped before it can be delivered.");
            Status = OrderStatus.Delivered;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Shipped)
                throw new InvalidOperationException($"Cannot cancel order with status '{Status}'.");
            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot cancel already canceled order.");
            Status = OrderStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool BelongsTo(Guid customerId) => CustomerId == customerId;

        private void RecalculateTotal()
        {
            if(_items.Count == 0)
            {
                TotalAmount = Money.Zero(Currency.USD);
                return;
            }
            var currency = _items.First().UnitPrice.Currency;
            TotalAmount = _items.Aggregate(
                Money.Zero(currency),
                (sum, item) => sum.Add(item.LineTotal));
        }
    }
}
