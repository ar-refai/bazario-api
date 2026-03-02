using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Customers.Dtos
{
    public sealed record class CustomerDto
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public bool IsAdmin { get; init; } 
        public AddressDto? ShippingAddress { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public sealed record AddressDto
    {
        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
    }
}
