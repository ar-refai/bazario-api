using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Aggregates.Carts;
using ECommerceApi.Domain.Repositories;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Repositories
{
    public sealed class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        public CartRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<ShoppingCart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.ShoppingCarts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task<ShoppingCart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        => await _context.ShoppingCarts.Include(c => c.Items).FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);

        public async Task AddAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
        => await _context.ShoppingCarts.AddAsync(cart, cancellationToken);

        public void Update(ShoppingCart cart)
        => _context.ShoppingCarts.Update(cart);
    }
}
