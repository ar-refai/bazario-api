using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;

namespace ECommerceApi.Domain.ValueObjects
{
    public sealed class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string Country { get; }
        public string PostalCode { get; }

        private Address()
        {
            Street = string.Empty;
            City = string.Empty;
            Country = string.Empty;
            PostalCode = string.Empty;
        }

        public Address(string street, string city, string country, string postalCode)
        {
            if (string.IsNullOrEmpty(street))
                throw new ArgumentException("Street is required.", nameof(street));
            if (string.IsNullOrEmpty(city)) 
                throw new ArgumentException("City is required.", nameof(city));
            if (string.IsNullOrEmpty(country))
                throw new ArgumentException("Country is required.", nameof(country));
            if(string.IsNullOrEmpty(postalCode))
                throw new ArgumentException("Postal Code is required.",nameof(postalCode));
            Street = street;
            City = city;
            Country = country;
            PostalCode = postalCode;
        }
        
        protected override IEnumerable<object?> getEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return Country;
            yield return PostalCode;
        }

        public override string ToString()
        {
            return $"{Street}, {City}, {Country}, {PostalCode}";
        }
    }
}
