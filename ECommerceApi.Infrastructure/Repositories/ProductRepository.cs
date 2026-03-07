using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerceApi.Domain.Aggregates.Products;
using ECommerceApi.Domain.Repositories;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Repositories
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Products.OrderBy(p => p.Category).ThenBy(p => p.Name).ToListAsync(cancellationToken);

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            var normalize = sku.Trim().ToLowerInvariant();
            return await _context.Products.FirstOrDefaultAsync(p => p.Sku.Value == normalize, cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> SearchAsync(string? nameFilter, string? category, decimal? minPrice, decimal? maxPrice, CancellationToken cancellationToken = default)
        {
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(nameFilter))
                query.Where(p => p.Name.Contains(nameFilter));
            if(!string.IsNullOrWhiteSpace(category))
                query.Where(p => p.Category.Contains(category));
            if(minPrice.HasValue)
                query.Where(p => p.Price.Amount > minPrice.Value);
            if (maxPrice.HasValue)
                query.Where(p => p.Price.Amount < maxPrice.Value);
            return await query.OrderBy(p => p.Price.Amount).ToListAsync(cancellationToken);

        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        => await _context.Products.AddAsync(product, cancellationToken);

        public void Update(Product product)
        => _context.Products.Update(product);


        public void Delete(Product product)
        => _context.Products.Remove(product);
    }
}
