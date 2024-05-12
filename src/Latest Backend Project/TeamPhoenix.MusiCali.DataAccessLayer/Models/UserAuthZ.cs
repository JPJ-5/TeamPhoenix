using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class UserAuthZ
    {
        public string? Username { get; set; }
        public string? Salt { get; set; }
        public string? Password { get; set; }

        public List<UserPermission> Permissions { get; set; } = new List<UserPermission>();
        public List<UserRole> Roles { get; set; } = new List<UserRole>();

        public DateTime RegistrationTimestamp { get; set; }
        public DateTime LastLoginTimestamp { get; set; }
        public DateTime LastActivityTimestamp { get; set; }

        public List<UserRole> GetUserRoles()
        {
            return Roles;
        }

        public List<UserPermission> GetUserPermissions()
        {
            return Permissions;
        }
        


        public bool IsAuthorize(string userIdentity, string securityContext)
        {
            //Change later with actual logic
            return true;
        }
    }

}

public enum UserRole
{
    User,
    Admin,
}

public enum UserPermission
{
    AccessLimitedFeatures,
    AccessRegisteredUserFeatures,
    AccessAuthenticatedUserFeatures,
}

public enum RegistrationStatus
{
    Incomplete,
    Completed
}