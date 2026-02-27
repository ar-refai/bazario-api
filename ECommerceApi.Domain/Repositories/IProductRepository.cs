using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Aggregates.Products;

namespace ECommerceApi.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default );
        Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default );
        Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> SearchAsync(
            string? nameFilter,
            string? category,
            decimal? minPrice,
            decimal? maxPrice,
            CancellationToken cancellationToken = default);
        Task AddAsync(Product product ,CancellationToken cancellationToken = default);
        Task Update(Product product);
        Task Delete(Product product);

    }
}
