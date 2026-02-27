using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.ValueObjects;

namespace ECommerceApi.Domain.Aggregates.Customers
{
    public sealed class Customer : AggregateRoot
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;

        public Email Email { get; private set; } = null!;

        public string PasswordHash { get; private set; } = string.Empty;
        public Address? ShippingAddress { get; private set; }

        public bool IsAdmin { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public string FullName => FirstName + " " + LastName;

        private Customer() { }
        private Customer(Guid id ,string firstName, string lastName, Email email, string passwordHash) : base (id)
        {
            {
                FirstName = firstName;
                LastName = lastName;
                Email = email;
                PasswordHash = passwordHash;
                IsAdmin = false;
                CreatedAt = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        public static Customer Create(string firstName, string lastName, string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.", nameof(lastName));
            return new Customer
            (
                id : Guid.NewGuid(),
                firstName : firstName.Trim(),
                lastName : lastName.Trim(),
                email: new Email(email),
                passwordHash: passwordHash
            );
        }

        public void UpdateProfile(string firstName, string lastName, Address? shippingAddress)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.", nameof(lastName));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            ShippingAddress =  shippingAddress;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow; 
        }

        public void PromoteToAdmin()
        {
            IsAdmin = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
