using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Auth.Commands
{
    public sealed record class LoginCommand(string Email, string Password);
}
