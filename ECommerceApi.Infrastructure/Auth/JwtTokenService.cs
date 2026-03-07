using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ECommerceApi.Application.Auth.Dtos;
using ECommerceApi.Application.Common;
using ECommerceApi.Domain.Aggregates.Customers;
using ECommerceApi.Domain.Repositories;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceApi.Infrastructure.Auth;

public sealed class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    private readonly ICustomerRepository _customerRepository;

    public JwtTokenService(
        IConfiguration configuration,
        AppDbContext context,
        ICustomerRepository customerRepository)
    {
        _configuration = configuration;
        _context = context;
        _customerRepository = customerRepository;
    }

    public async Task<AuthResponseDto> GenerateTokensAsync(
        Customer customer,
        CancellationToken cancellationToken = default)
    {
        var accessToken = GenerateAccessToken(customer);
        var refreshToken = await GenerateAndStoreRefreshTokenAsync(customer.Id, cancellationToken);

        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            CustomerId = customer.Id,
            Email = customer.Email.Value,
            IsAdmin = customer.IsAdmin
        };
    }

    public async Task<Result<AuthResponseDto>> RefreshTokensAsync(
        string refreshTokenValue,
        CancellationToken cancellationToken = default)
    {
        // 1. Find the token in the database
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshTokenValue, cancellationToken);

        if (storedToken is null)
            return Error.Unauthorized("Invalid refresh token.");

        // 2. Check if it's been revoked or expired
        if (!storedToken.IsActive)
        {
            // If the token is revoked (not just expired), this could be a reuse attack.
            // Revoke ALL tokens for this customer — force them to log in again.
            if (storedToken.IsRevoked)
            {
                await RevokeAllCustomerTokensAsync(storedToken.CustomerId, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Error.Unauthorized("Refresh token is expired or revoked.");
        }

        // 3. Load the customer
        var customer = await _customerRepository
            .GetByIdAsync(storedToken.CustomerId, cancellationToken);

        if (customer is null)
            return Error.Unauthorized("Customer not found.");

        // 4. Generate new tokens
        var newAccessToken = GenerateAccessToken(customer);
        var newRefreshToken = await GenerateAndStoreRefreshTokenAsync(
            customer.Id, cancellationToken);

        // 5. Revoke the old refresh token (rotation — old token can never be used again)
        storedToken.Revoke(replacedByToken: newRefreshToken.Token);
        _context.RefreshTokens.Update(storedToken);

        await _context.SaveChangesAsync(cancellationToken);

        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            CustomerId = customer.Id,
            Email = customer.Email.Value,
            IsAdmin = customer.IsAdmin
        };
    }

    // ── Private Helpers ───────────────────────────────────────────────────

    private string GenerateAccessToken(Customer customer)
    {
        var secretKey = _configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is not configured.");
        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT Audience is not configured.");
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        // Claims embedded in the token payload.
        // These are decoded by the API on every request — no database lookup needed.
        var claims = new[]
        {
            // sub (subject): standard JWT claim for the user's unique identifier
            new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),

            // jti (JWT ID): unique ID for this specific token — useful for revocation
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // Email in the token so the API can display it without a DB query
            new Claim(JwtRegisteredClaimNames.Email, customer.Email.Value),

            // Custom role claim — maps to [Authorize(Roles = "Admin")]
            new Claim(ClaimTypes.Role, customer.IsAdmin ? "Admin" : "Customer"),

            // Store the full name for display purposes
            new Claim(ClaimTypes.Name, customer.FullName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> GenerateAndStoreRefreshTokenAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var expiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");

        // Cryptographically random token — not predictable, not guessable.
        var tokenBytes = RandomNumberGenerator.GetBytes(64);
        var tokenValue = Convert.ToBase64String(tokenBytes);

        var refreshToken = new RefreshToken(
            customerId,
            tokenValue,
            DateTime.UtcNow.AddDays(expiryDays));

        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        return refreshToken;
    }

    private async Task RevokeAllCustomerTokensAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(t => t.CustomerId == customerId && !t.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
            token.Revoke();
    }
}