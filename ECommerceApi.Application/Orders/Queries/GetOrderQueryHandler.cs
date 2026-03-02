using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Application.Common;
using ECommerceApi.Application.Orders.Dtos;
using ECommerceApi.Domain.Repositories;

namespace ECommerceApi.Application.Orders.Queries
{
    public sealed class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepo;
        public GetOrderQueryHandler(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<Result<OrderDto>> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepo.GetByIdAsync(query.OrderId, cancellationToken);
            if (order is null)
                return Error.NotFound("Order", query.OrderId);

            if (!query.IsAdmin && !order.BelongsTo(query.RequestingCustomerId))
                return Error.Forbidden();

            return order.ToDto(); 
        }
    }
}
