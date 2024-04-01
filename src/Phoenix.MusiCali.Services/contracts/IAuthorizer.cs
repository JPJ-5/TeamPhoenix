namespace Phoenix.MusiCali.Contracts;
public interface IAuthorizer
    {
        // Checks if a user is Authorized
        bool IsAuthorize(string userIdentity, string securityContext);
        
         // Retrieve a user by their username
        User GetUserByUsername(string username);

        // Retrieve user roles for a given user
        List<UserRole> GetUserRoles(UserAuth user);

        // Retrieve user permissions for a given user
        List<UserPermission> GetUserPermissions(UserAuth user);

        // Save changes to a user
        void SaveUser(UserAuth user);

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