using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Aggregates.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceApi.Infrastructure.Persistence.Configurations
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.CustomerId).IsRequired();

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(o => o.CreatedAt).IsRequired();
            builder.Property(o => o.UpdatedAt).IsRequired();

            // ── OrderNumber ───────────────────────────────────────────────────
            builder.OwnsOne(o => o.OrderNumber, nb =>
            {
                nb.Property(n => n.Value)
                    .HasColumnName("OrderNumber")
                    .HasMaxLength(25)
                    .IsRequired();

                nb.HasIndex(n => n.Value)
                    .IsUnique()
                    .HasDatabaseName("IX_Orders_OrderNumber");
            });

            // ── ShippingAddress ───────────────────────────────────────────────
            builder.OwnsOne(o => o.ShippingAddress, ab =>
            {
                ab.Property(a => a.Street)
                    .HasColumnName("ShippingAddress_Street")
                    .HasMaxLength(250)
                    .IsRequired();

                ab.Property(a => a.City)
                    .HasColumnName("ShippingAddress_City")
                    .HasMaxLength(100)
                    .IsRequired();

                ab.Property(a => a.Country)
                    .HasColumnName("ShippingAddress_Country")
                    .HasMaxLength(100)
                    .IsRequired();

                ab.Property(a => a.PostalCode)
                    .HasColumnName("ShippingAddress_PostalCode")
                    .HasMaxLength(20)
                    .IsRequired();
            });

            // ── TotalAmount ───────────────────────────────────────────────────
            // OwnsOne at root level — HasPrecision works fine here.
            builder.OwnsOne(o => o.TotalAmount, mb =>
            {
                mb.Property(m => m.Amount)
                    .HasColumnName("TotalAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                mb.Property(m => m.Currency)
                    .HasColumnName("TotalAmount_Currency")
                    .HasConversion<string>()
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // ── OrderItems ────────────────────────────────────────────────────
            builder.OwnsMany(o => o.Items, ib =>
            {
                ib.ToTable("OrderItems");

                ib.WithOwner().HasForeignKey("OrderId");

                ib.HasKey(i => i.Id);

                ib.Property(i => i.ProductId).IsRequired();

                ib.Property(i => i.ProductName)
                    .HasMaxLength(200)
                    .IsRequired();

                ib.Property(i => i.Quantity).IsRequired();

                ib.Ignore(i => i.LineTotal);

                // OwnsOne nested inside OwnsMany — use HasColumnType instead of
                // HasPrecision to avoid the scale mis-application bug in EF Core 9.
                ib.OwnsOne(i => i.UnitPrice, mb =>
                {
                    mb.Property(m => m.Amount)
                        .HasColumnName("UnitPrice")
                        .HasColumnType("decimal(18,2)")   // ← explicit type, not HasPrecision
                        .IsRequired();

                    mb.Property(m => m.Currency)
                        .HasColumnName("UnitPrice_Currency")
                        .HasConversion<string>()
                        .HasMaxLength(3)
                        .IsRequired();
                });
            });

            builder.Navigation(o => o.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }

    }
}
