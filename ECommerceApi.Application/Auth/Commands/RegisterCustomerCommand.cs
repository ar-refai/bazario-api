using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Auth.Commands
{
    public sealed record class RegisterCustomerCommand(string FirstName, string LastName, string Email, string Password);
    
}
