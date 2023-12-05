namespace Services{
    using Phoenix.MusiCali.Models;
    public class Authorization
    {
        private readonly IUserRepository _userRepository;
    
        public Authorization(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
    
        public bool AuthorizeUser(User user, UserPermission requiredPermission)
        {
            List<UserPermission> userPermissions = _userRepository.GetUserPermissions(user);
    
            return userPermissions.Contains(requiredPermission);
        }
    
        public bool UserHasRole(User user, UserRole requiredRole)
        {
            List<UserRole> userRoles = _userRepository.GetUserRoles(user);
    
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
