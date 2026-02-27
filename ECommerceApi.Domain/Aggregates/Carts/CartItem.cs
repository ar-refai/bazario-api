using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.ValueObjects;

namespace ECommerceApi.Domain.Aggregates.Carts
{
    public class CartItem : Entity
    {
        public Guid CartId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public Money UnitPrice { get; private set; } = null!;
        public int Quantity { get; private set; }

        public Money LineTotal => UnitPrice.Multiply(Quantity);

        private CartItem()
        {
            
        }

        internal CartItem(Guid cartId, Guid productId, string productName, Money unitPrice, int quantity) 
            : base(Guid.NewGuid())
        {
            CartId = cartId;
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        /// <summary>
        /// Sets the quantity to a new positive value.
        /// </summary>
        /// <param name="newQuantity">The new quantity value to set. Must be positive.</param>
        /// <exception cref="ArgumentException">Thrown when newQuantity is less than or equal to zero.</exception>
        internal void UpdateQuantity(int newQuantity)
        {
            if(newQuantity <= 0)
                throw new ArgumentException("Quantity must be positive.", nameof(newQuantity));
            Quantity = newQuantity;
        }
    }
}
