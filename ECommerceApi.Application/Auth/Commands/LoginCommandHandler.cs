using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Application.Auth.Dtos;
using ECommerceApi.Application.Common;
using ECommerceApi.Domain.Repositories;

namespace ECommerceApi.Application.Auth.Commands
{
    public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponseDto>
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(ICustomerRepository customerRepo, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _customerRepo = customerRepo;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResponseDto>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepo.GetByEmailAsync(command.Email,cancellationToken);
            if (customer is null)
                return Error.Unauthorized("Invalid email or password");

            var passwordValid = _passwordHasher.Verify(command.Password, customer.PasswordHash);
            if (!passwordValid)
                return Error.Unauthorized("Invalid email or password");

            var tokenResult = await _tokenService.GenerateTokenAsync(customer, cancellationToken);
            return tokenResult;
        }
    }
}
