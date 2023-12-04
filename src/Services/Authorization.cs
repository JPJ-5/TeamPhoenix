namespace Services{
    using Phoenix.MusiCali.Models;
    public class Authorization
    {
        private readonly IUserRepository _userRepository;
    
        public AuthorizationService(IUserRepository userRepository)
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
    
        public bool IsUnregisteredUser(User user)
        {
            // Logic to check if the user is an Unregistered User
            return return user.RegistrationStatus == RegistrationStatus.Incomplete;
        }
    
        public bool IsRegisteredUser(User user)
        {
            // Logic to check if the user is a Registered User
            return user.RegistrationStatus == RegistrationStatus.Completed;;
        }
    
        public bool IsAuthenticatedUser(User user)
        {
            // Logic to check if the user is an Authenticated User
            return user.IsLoggedIn && user.HasActiveSession;;
        }
    }
}
