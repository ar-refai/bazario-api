using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Application.Common;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Repositories;

namespace ECommerceApi.Application.Orders.Commands
{
    public sealed class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWord;
        public CancelOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWord = unitOfWork;
        }

        public async Task<Result> HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken = default)
        {

            var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
            if (order is null)
                return Result.Failure(Error.NotFound("Order", command.OrderId));

            if (!command.IsAdmin && !order.BelongsTo(command.RequestingCustomerId))
                return Result.Failure(Error.Forbidden());

            try
            {
                order.Cancel();
            }catch(InvalidOperationException ex)
            {
                return Result.Failure(Error.BusinessRule("Order.CannotCancel",ex.Message));
            }

            // restore each item in the canceled orders
            foreach(var item in order.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                // Product might be deleted — still restore stock if it exists
                if (product is not null)
                {
                    product.RestoreStock(item.Quantity);
                    _productRepository.Update(product);
                }
            }

            _orderRepository.Update(order);
            await _unitOfWord.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }
    }
}
