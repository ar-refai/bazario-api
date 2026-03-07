using ECommerceApi.Application.Common;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Repositories;
using ECommerceApi.Infrastructure.Auth;
using ECommerceApi.Infrastructure.Persistence;
using ECommerceApi.Infrastructure.Persistence.Interceptors;
using ECommerceApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerceApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Interceptors ─────────────────────────────────────────────────
        // Registered as singleton — interceptors are stateless.
        // Must be registered before DbContext so EF Core picks them up.
        services.AddSingleton<AuditInterceptor>();

        // ── DbContext ─────────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            options.UseSqlServer(connectionString, sqlOptions =>
            {
                // Retry on transient failures (network blips, SQL Server restart).
                // 3 retries with exponential backoff.
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });

            // Add the audit interceptor to this DbContext instance.
            var auditInterceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
            options.AddInterceptors(auditInterceptor);
        });

        // ── Unit of Work ──────────────────────────────────────────────────
        // AppDbContext implements IUnitOfWork. When the Application layer
        // requests IUnitOfWork, it gets the same AppDbContext instance
        // (scoped lifetime — one per HTTP request).
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // ── Repositories ──────────────────────────────────────────────────
        // Scoped: new instance per HTTP request. Shares the same DbContext
        // within a request, so all repository operations participate in
        // the same EF Core change tracker and transaction.
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

        // ── Auth Services ─────────────────────────────────────────────────
        // Singleton: password hasher is stateless — one instance is fine.
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        // Scoped: token service accesses DbContext, which is scoped.
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
}