using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Aggregates.Carts;
using ECommerceApi.Domain.Aggregates.Customers;
using ECommerceApi.Domain.Aggregates.Orders;
using ECommerceApi.Domain.Aggregates.Products;
using ECommerceApi.Domain.Common;
using ECommerceApi.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Persistence
{
    public sealed class AppDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();

        // RefreshToken is infrastructure-only — not a domain aggregate.
        // But it needs persistence, so it lives in the DbContext.

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Discovers and applies all IEntityTypeConfiguration<T> implementations
            // in the Infrastructure assembly automatically.

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

    }
}
