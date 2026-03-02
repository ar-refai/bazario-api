using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Auth.Dtos
{
    public sealed record class AuthResponseDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;

        // Tell the client when the access token expires so it can
        // schedule a refresh before the token becomes invalid
        public DateTime ExpiresAt { get; init; }

        public Guid CustomerId { get; init; }
        public string Email { get; init; } = string.Empty;
        public bool IsAdmin { get; init; }
    }
}
