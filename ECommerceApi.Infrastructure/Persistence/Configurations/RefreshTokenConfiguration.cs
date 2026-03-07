using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceApi.Infrastructure.Persistence.Configurations
{
    public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.CustomerId).IsRequired();
            builder.Property(r => r.Token).HasMaxLength(500).IsRequired();
            builder.Property(r => r.ReplacedByToken).HasMaxLength(500);
            builder.Property(r => r.ExpiresAt).IsRequired();
            builder.Property(r => r.IsRevoked).IsRequired();
            builder.Property(r => r.CreatedAt).IsRequired();
            builder.HasIndex(r => r.Token).HasDatabaseName("IX_RefreshToken_Token");
            builder.HasIndex(r => r.CustomerId).HasDatabaseName("IX_RefreshToken_CustomerId");

        }
    }
}
