using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Security.Contract
{
    public interface IAuthorizer
    {
        // Checks if a user is Authorized
        bool IsAuthorize(string userIdentity, string securityContext);

        // Retrieve user roles for a given user
        List<UserRole> GetUserRoles(UserAuthN user);

        // Retrieve user permissions for a given user
        List<UserPermission> GetUserPermissions(UserAuthN user);

    }
}
