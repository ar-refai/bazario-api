using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.ValueObjects;

namespace ECommerceApi.Domain.Aggregates.Orders
{
    public sealed class OrderItem : Entity
    {
        // order id , product id , product name , unit price , quantity , Computed(LineTotal => )
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public Money UnitPrice { get; private set; } = null!;
        public int Quantity { get; private set; }
        public Money LineTotal => UnitPrice.Multiply(Quantity);
        private OrderItem () { }
        internal OrderItem (Guid orderId, Guid productId, string productName, Money unitPrice, int quantity) : base(Guid.NewGuid())
        {
            OrderId = orderId;
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

    }
}
