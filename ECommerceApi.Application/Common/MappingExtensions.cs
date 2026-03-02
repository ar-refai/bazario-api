using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Application.Carts.Dtos;
using ECommerceApi.Application.Customers.Dtos;
using ECommerceApi.Application.Orders.Dtos;
using ECommerceApi.Application.Products.Dtos;
using ECommerceApi.Domain.Aggregates.Carts;
using ECommerceApi.Domain.Aggregates.Customers;
using ECommerceApi.Domain.Aggregates.Orders;
using ECommerceApi.Domain.Aggregates.Products;
using ECommerceApi.Domain.ValueObjects;

namespace ECommerceApi.Application.Common
{
    public static class MappingExtensions
    {
        // *** Product ******************************************
        public static ProductDto ToDto(this Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency.ToString(),
            Category = product.Category,
            StockQuantity = product.StockQuantity,
            Sku = product.Sku.Value,
            IsDelted = product.IsDeleted,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        // *** Order *********************************************
        public static OrderDto ToDto(this Order order) => new()
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber.Value,
            Status = order.Status.ToString(),
            ShippingAddress = order.ShippingAddress.ToDto(),
            Items = order.Items.Select(i => i.ToDto()).ToList(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };

        public static OrderItemDto ToDto(this OrderItem orderItem) => new()
        {
            Id = orderItem.Id,
            ProductName = orderItem.ProductName,
            UnitPrice = orderItem.UnitPrice.Amount,
            Currency = orderItem.UnitPrice.Currency.ToString(),
            Quantity = orderItem.Quantity,
            LineTotal = orderItem.LineTotal.Amount
        };

        public static Orders.Dtos.AddressDto ToDto(this Address address) => new()
        {
            Street = address.Street,
            City = address.City,
            Country = address.Country,
            PostalCode = address.PostalCode
        };

        // *** Customer *********************************************
        public static CustomerDto ToDto(this Customer customer) => new()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email.Value,
            IsAdmin = customer.IsAdmin,
            ShippingAddress = customer.ShippingAddress?.CustomerAddressToDto(),
            CreatedAt = customer.CreatedAt
        };

        public static Customers.Dtos.AddressDto CustomerAddressToDto(this Address address) => new()
        {
            Street = address.Street,
            City = address.City,
            Country = address.Country,
            PostalCode = address.PostalCode
        };

        // *** Cart *********************************************
        public static CartDto ToDto(this ShoppingCart cart) => new()
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,
            Items = cart.Items.Select(i => i.ToDto()).ToList(),
            Total = cart.Total.Amount,
            Currency = cart.Items.Any() ? cart.Items.First().UnitPrice.Currency.ToString() : "USD",
        };

        public static CartItemDto ToDto(this CartItem item) => new()
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            UnitPrice = item.UnitPrice.Amount,
            Currency = item.UnitPrice.Currency.ToString(),
            Quantity = item.Quantity,
            LineTotal = item.LineTotal.Amount
        };

    }
}
