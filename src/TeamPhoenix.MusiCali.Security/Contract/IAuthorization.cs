using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

// IAuthorization.cs
namespace TeamPhoenix.MusiCali.Security.Contracts
{
    public interface IAuthorization
    {
        bool IsUserAuthorized(ClaimsPrincipal userPrincipal, string resource, string action);
    }
}
