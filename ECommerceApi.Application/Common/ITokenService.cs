using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Application.Auth.Dtos;
using ECommerceApi.Domain.Aggregates.Customers;

namespace ECommerceApi.Application.Common
{
    public interface ITokenService
    {
        Task<AuthResponseDto> GenerateTokenAsync(Customer customer, CancellationToken cancellationToken = default);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
