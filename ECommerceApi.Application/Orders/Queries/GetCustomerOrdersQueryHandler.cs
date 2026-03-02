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
    public sealed class GetCustomerOrdersQueryHandler : IQueryHandler<GetCustomerOrdersQuery, IReadOnlyList<OrderDto>>
    {
        private readonly IOrderRepository _orderRepo;
        public GetCustomerOrdersQueryHandler(IOrderRepository orederRepo)
        {
            _orderRepo = orederRepo;
        }

        public async Task<Result<IReadOnlyList<OrderDto>>> HandleAsync(GetCustomerOrdersQuery query, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepo.GetByCustomerIdAsync(query.CustomerId,cancellationToken);
            return orders.Select(o => o.ToDto()).ToList();
        }
    }
}
