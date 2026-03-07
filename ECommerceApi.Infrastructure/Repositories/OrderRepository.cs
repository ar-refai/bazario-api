    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Aggregates.Orders;
using ECommerceApi.Domain.Repositories;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Orders.Include(o => o.Items).OrderByDescending(o => o.CreatedAt).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        => await _context.Orders.Include(o => o.Items).Where(o => o.CustomerId == customerId).OrderByDescending(o => o.CreatedAt).ToListAsync(cancellationToken);

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id ,cancellationToken);

        public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        => await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderNumber.Value == orderNumber, cancellationToken);

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        => await _context.Orders.AddAsync(order, cancellationToken);

        public void Update(Order order)
        => _context.Orders.Update(order);
    }
}
