namespace Phoenix.MusiCali.Services{
    using Phoenix.MusiCali.Models;
    public class Authorization
    {
        private readonly IAuthorizer _authorizer;
    
        public Authorization(IAuthorizer authorizer)
        {
            _authorizer = authorizer ?? throw new ArgumentNullException(nameof(authorizer));
        }
    
        public bool AuthorizeUser(UserAuth user, UserPermission requiredPermission)
        {
            List<UserPermission> userPermissions = _authorizer.GetUserPermissions(user);
    
            return userPermissions.Contains(requiredPermission);
        }
    
        public bool UserHasRole(UserAuth userAuth, UserRole requiredRole)
        {
            List<UserRole> userRoles = userAuth.GetUserRoles(userAuth);
    
            return userRoles.Contains(requiredRole);
        }
    
        public bool IsUnregisteredUser(UserAuth user)
        {
            //Checks if the user is an Unregistered User
            return user.GetRegistrationStatus() == RegistrationStatus.Incomplete;
        }
    
        public bool IsRegisteredUser(UserAuth user)
        {
            //Checks if the user is a Registered User
            return user.GetRegistrationStatus() == RegistrationStatus.Completed;;
        }
    
        public bool IsAuthenticatedUser(UserAuth user)
        {
            //Checks if the user is an Authenticated User
            return user.IsLoggedIn() && user.HasActiveSession();
        }
    }
}
