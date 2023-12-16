namespace Phoenix.MusiCali.Services{
    using Phoenix.MusiCali.Models;
    public class Authorization
    {
        private readonly IAuthorizer _authorizer;
    
        public Authorization(IAuthorizer authorizer)
        {
            _authorizer = authorizer ?? throw new ArgumentNullException(nameof(authorizer));
        }
    
        public bool AuthorizeUser(UserAuthZ user, UserPermission requiredPermission)
        {
            List<UserPermission> userPermissions = _authorizer.GetUserPermissions(user);
    
            return userPermissions.Contains(requiredPermission);
        }
    
        public bool UserHasRole(UserAuthZ userAuth, UserRole requiredRole)
        {
            List<UserRole> userRoles = userAuth.GetUserRoles(userAuth);
    
            return userRoles.Contains(requiredRole);
        }
    
        public bool IsUnregisteredUser(UserAuthZ user)
        {
            //Checks if the user is an Unregistered User
            return user.GetRegistrationStatus() == RegistrationStatus.Incomplete;
        }
    
        public bool IsRegisteredUser(UserAuthZ user)
        {
            //Checks if the user is a Registered User
            return user.GetRegistrationStatus() == RegistrationStatus.Completed;;
        }
    
        public bool IsAuthenticatedUser(UserAuthZ user)
        {
            //Checks if the user is an Authenticated User
            return user.IsLoggedIn() && user.HasActiveSession();
        }
    }
}
