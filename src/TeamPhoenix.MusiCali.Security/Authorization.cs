using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.Security.Contracts;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security.Contracts;
using System.Net;

namespace TeamPhoenix.MusiCali.Security
{
    public class Authorization : IAuthorization
    {
        public bool IsUserAuthorized(ClaimsPrincipal userPrincipal, string resource, string action)
        {
            if (userPrincipal.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin")
            {
                // User is authorized
                return true;
            }

            // User is not authorized
            return false;
        }
    }
}
