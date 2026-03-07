using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Aggregates.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceApi.Infrastructure.Persistence.Configurations
{
    public sealed class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.ToTable("ShoppingCart");
            builder.HasKey(sc => sc.Id);
            builder.Property(sc => sc.CustomerId).IsRequired();
            builder.Property(sc => sc.CreatedAt).IsRequired();
            builder.Property(sc => sc.UpdatedAt).IsRequired();
            builder.Ignore(sc => sc.Total);
            builder.HasIndex(sc => sc.CustomerId).HasDatabaseName("IX_ShoppingCart_CustomerId");
            builder.OwnsMany(sc => sc.Items, ciBuilder =>
            {
                ciBuilder.ToTable("CartItems");
                ciBuilder.HasKey(ci => ci.Id);
                ciBuilder.WithOwner().HasForeignKey("CartId");
                ciBuilder.Property(ci => ci.ProductId).IsRequired();
                ciBuilder.Property(ci => ci.ProductName).HasMaxLength(200).IsRequired();
                ciBuilder.Property(ci => ci.Quantity).IsRequired();
                ciBuilder.Ignore(ci => ci.LineTotal);
                ciBuilder.OwnsOne(ci => ci.UnitPrice, upBuilder =>
                {
                    upBuilder.Property(up => up.Amount).HasColumnName("UnitPrice").HasColumnType("decimal(18,2)").IsRequired();
                    upBuilder.Property(up => up.Currency).HasColumnName("UnitPrice_Currency").HasConversion<string>().HasMaxLength(3).IsRequired();
                });
            });
            builder.Navigation(sc => sc.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}
