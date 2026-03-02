using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Application.Common;
using ECommerceApi.Domain.Aggregates.Carts;
using ECommerceApi.Domain.Aggregates.Customers;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Repositories;

namespace ECommerceApi.Application.Auth.Commands
{
    public sealed class RegisterCustomerCommandHandler : ICommandHandler<RegisterCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork; 

        public RegisterCustomerCommandHandler(ICustomerRepository customerRepo, ICartRepository cartRepo, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            _customerRepo = customerRepo;
            _cartRepo = cartRepo;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> HandleAsync(RegisterCustomerCommand command, CancellationToken cancellationToken = default)
        {
            // 1. check if email duplications
            var emailExists = await _customerRepo.ExistsByEmailAsync(command.Email, cancellationToken);
            if (emailExists)
                return Error.Conflict("Customer", $"A customer with email '{command.Email}' already exists.");

            // 2. hash the password 
            var passwordHash = _passwordHasher.Hash(command.Password);

            // 3. create customer 
            Customer customer;
            try
            {
                customer = Customer.Create(
                    command.FirstName,
                    command.LastName,
                    command.Email,
                    passwordHash
                    );
            }

            catch (ArgumentException ex) {
                return Error.Validation(ex.ParamName ?? "Input", ex.Message);
            }

            // 4. create customer shopping cart
            var cart = ShoppingCart.Create(customer.Id);

            // 5. presist
            await _customerRepo.AddAsync(customer, cancellationToken);
            await _cartRepo.AddAsync(cart, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return customer.Id;
        }
    }
}
