using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Aggregates.Carts;

namespace ECommerceApi.Domain.Repositories
{
    public interface ICartRepository
    {
        Task<ShoppingCart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ShoppingCart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task AddAsync(ShoppingCart cart, CancellationToken cancellationToken = default);
        void Update(ShoppingCart cart);
    }
}
