using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Application.Common;
using ECommerceApi.Domain.Aggregates.Carts;
using ECommerceApi.Domain.Aggregates.Orders;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Repositories;
using ECommerceApi.Domain.ValueObjects;

namespace ECommerceApi.Application.Orders.Commands
{
    public sealed class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand, Guid>
    {

        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PlaceOrderCommandHandler(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Processes a place order command by validating the cart, reserving product stock, creating an order, and
        /// persisting changes.
        /// </summary>
        /// <param name="command">The command containing order and shipping details.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A result containing the created order's unique identifier or an error if the operation fails.</returns>
        public async Task<Result<Guid>> HandleAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetByIdAsync(command.CustomerId, cancellationToken);
            if (cart is null)
                return Error.NotFound("Cart","CustomerId",command.CustomerId);

            if (!cart.Items.Any())
                return Error.BusinessRule("Cart.Empty", "Cannot place order with empty cart.");

            if (!cart.BelongsTo(command.CustomerId))
                return Error.Forbidden();

            // build the shipping address value object 
            Address shippingAddress;
            try
            {
                shippingAddress = new(command.Street, command.City, command.Country, command.PostalCode);
            }
            catch (ArgumentException ex) 
            {
                return Error.Validation(ex.ParamName ?? "Address", ex.Message);
            }

            // create the order
            var order = Order.Create(command.CustomerId, shippingAddress);

            // loop over the cart items one by one
            foreach(var item in cart.Items)
            {
                // get product by the id in the item id, check if it is null and check if it is deleted
                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product is null)
                    return Error.NotFound("Product", item.ProductId);

                if (product.IsDeleted)
                    return Error.BusinessRule(
                        "Product.Unavailable",
                        $"Product '{product.Name}' is no longer available.");

                // reserve the stock in a try catch block to catch the predicted exception of  InsufficientStock       
                try
                {
                    product.ReserveStock(item.Quantity);
                }catch(InvalidOperationException ex)
                {
                    return Error.BusinessRule("Product.InsufficientStock", ex.Message);
                }

                // add item to the order with price snapshot from the product
                order.AddItem(
                    product.Id,
                    product.Name,
                    product.Price,
                    item.Quantity
                    );

                // update the product as there were a newQuantity subtracted from it's quantity
                _productRepository.Update(product);
            }

            order.Place();

            cart.ClearCart();

            // persist all changes in a single transaction
            await _orderRepository.AddAsync(order, cancellationToken);
            _cartRepository.Update(cart);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return order.Id;
        }
    }
}
