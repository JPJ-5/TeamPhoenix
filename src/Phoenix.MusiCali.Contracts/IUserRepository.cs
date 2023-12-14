using System.Collections.Generic;
using System.Net.Security;

namespace Phoenix.MusiCali.Contracts
{
    public interface IUserRepository
    {
        // Retrieve a user by their username
        User GetUserByUsername(string username);

        // Retrieve user roles for a given user
        List<UserRole> GetUserRoles(User user);

        // Retrieve user permissions for a given user
        List<UserPermission> GetUserPermissions(User user);

        // Save changes to a user (e.g., update roles, permissions)
        void SaveUser(User user);

    }

    

    /*

    if(auth.Authenticate("",""))
    {
        if(auth.IsAuthorize())
        {
            //Do stuff
        }
    }

    */
}