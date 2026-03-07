using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Infrastructure.Auth
{
    public sealed class RefreshToken
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public string Token { get; private set; } = string.Empty;

        // When this refresh token expires — independent of the access token.
        public DateTime ExpiresAt { get; private set; }

        // Revoked tokens are kept in the database for security audit.
        // We never delete them — we mark them revoked.
        public bool IsRevoked { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // If this token was used to generate a new one, we record which one.
        // This enables detecting token reuse attacks (if a revoked token is presented,
        // the entire token family is invalidated).
        public string? ReplacedByToken { get; private set; }

        private RefreshToken() { }
        public RefreshToken( Guid customerId, string token, DateTime expiresAt)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            Token = token;
            ExpiresAt = expiresAt;
            IsRevoked = false;
            CreatedAt = DateTime.UtcNow;
        }
        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;
        public void Revoke(string? replacedByToken = null)
        {
            IsRevoked = true;
            ReplacedByToken = replacedByToken;
        }
    }
}
