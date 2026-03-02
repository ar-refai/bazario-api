using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Application.Auth.Dtos;
using ECommerceApi.Application.Common;

namespace ECommerceApi.Application.Auth.Commands
{
    public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResponseDto>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
        => await _tokenService.RefreshTokenAsync(command.RefreshToken, cancellationToken);
    }
}
