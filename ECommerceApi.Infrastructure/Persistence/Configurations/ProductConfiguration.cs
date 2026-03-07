using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Aggregates.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceApi.Infrastructure.Persistence.Configurations
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(p => p.Description)
                .HasMaxLength(2000);
            builder.Property(p => p.Category)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(p => p.StockQuantity)
                .IsRequired();
            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            builder.Property(p => p.CreatedAt)
                .IsRequired();
            builder.Property(p => p.UpdatedAt)
                .IsRequired();

            // *** Money Value Object ********************
            builder.OwnsOne(p => p.Price, moneyBuilder =>
            {
                moneyBuilder.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasPrecision(18, 2)
                .IsRequired();

                moneyBuilder.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasConversion<string>()
                .HasMaxLength(3)
                .IsRequired();
            });

            // *** ProductSku Value Object ***************
            builder.OwnsOne(p => p.Sku, skuBuilder =>
            {
                skuBuilder.Property(s => s.Value)
                .HasColumnName("Sku")
                .IsRequired()
                .HasMaxLength(20);

                skuBuilder.HasIndex(s => s.Value)
                .IsUnique()
                .HasDatabaseName("IX_Products_Sku");
            });


            // This filter is applied to EVERY query on Product automatically.
            // result = await context.Products.ToListAsync()
            // silently becomes: WHERE IsDeleted = 0
            //
            // To bypass it (for admin restore): context.Products.IgnoreQueryFilters()
            builder.HasQueryFilter(p => !p.IsDeleted);

            // *** ProductVariant Children ***************
            builder.OwnsMany(p => p.Variants, varBuilder =>
            {
                varBuilder.ToTable("ProductVariants");
                varBuilder.HasKey(v => v.Id);
                varBuilder.Property(v => v.Attributes)
                    .HasMaxLength(500);
                varBuilder.Property(v => v.StockQuantity)
                    .IsRequired();
                varBuilder.OwnsOne(v => v.Sku, skuBuilder =>
                {
                    skuBuilder.Property(s => s.Value)
                    .HasColumnName("Sku")
                    .HasMaxLength(20)
                    .IsRequired();
                });

                varBuilder.OwnsOne(v => v.PriceModifier, mb =>
                {
                    mb.Property(m => m.Amount)
                        .HasColumnName("PriceModifier")
                        .HasColumnType("decimal(18,2)")   
                        .IsRequired();

                    mb.Property(m => m.Currency)
                        .HasColumnName("PriceModifier_Currency")
                        .HasConversion<string>()
                        .HasMaxLength(3)
                        .IsRequired();
                });
            });



        }
    }
}
