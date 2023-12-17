using Phoenix.MusiCali.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.MusiCali.Services.Contracts
{
    internal interface IAuthentication
    {
        Principal Authenticate(string username, string password);
    }
}
