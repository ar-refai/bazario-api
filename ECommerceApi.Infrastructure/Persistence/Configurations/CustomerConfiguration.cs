using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Aggregates.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceApi.Infrastructure.Persistence.Configurations
{
    public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(100);

            // Computed property — no backing column, EF must not try to map it.
            builder.Ignore(c => c.FullName);

            builder.Property(c => c.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.IsAdmin)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.UpdatedAt).IsRequired();

            // ── Email (Owned Value Object) ─────────────────────────────────────
            // OwnsOne maps Email's properties as columns on the Customers table.
            // We rename the column from the default "Email_Value" to just "Email".
            builder.OwnsOne(c => c.Email, emailBuilder =>
            {
                emailBuilder.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(320);

                // The unique index must be declared INSIDE the OwnsOne builder,
                // targeting the column name directly as a string.
                // Why inside? Because EF Core only knows the column name "Email"
                // in the context of the owned type builder where we defined it.
                // Declaring it outside (on the root builder) causes the conflict
                // you just hit — EF sees "Email" and doesn't know if you mean
                // the navigation or the column.
                emailBuilder.HasIndex(e => e.Value)
                    .IsUnique()
                    .HasDatabaseName("IX_Customers_Email");
            });

            // ── ShippingAddress (Owned Value Object, nullable) ─────────────────
            builder.OwnsOne(c => c.ShippingAddress, addressBuilder =>
            {
                addressBuilder.Property(a => a.Street)
                    .HasColumnName("ShippingAddress_Street")
                    .HasMaxLength(250);

                addressBuilder.Property(a => a.City)
                    .HasColumnName("ShippingAddress_City")
                    .HasMaxLength(100);

                addressBuilder.Property(a => a.Country)
                    .HasColumnName("ShippingAddress_Country")
                    .HasMaxLength(100);

                addressBuilder.Property(a => a.PostalCode)
                    .HasColumnName("ShippingAddress_PostalCode")
                    .HasMaxLength(20);
            });
        }
    }
}