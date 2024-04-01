<<<<<<< HEAD:src/Phoenix.MusiCali.Models/UserAuthZ.cs
using System;
=======
﻿using System;
>>>>>>> 72f693b5df2daec32f621445a51313f102d5003c:src/Latest Backend Project/TeamPhoenix.MusiCali.DataAccessLayer/Models/UserAuthZ.cs
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

<<<<<<< HEAD:src/Phoenix.MusiCali.Models/UserAuthZ.cs
namespace Phoenix.MusiCali.Models
=======
namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
>>>>>>> 72f693b5df2daec32f621445a51313f102d5003c:src/Latest Backend Project/TeamPhoenix.MusiCali.DataAccessLayer/Models/UserAuthZ.cs
{
    public class UserAuthZ
    {
        public string? Username { get; set; }
<<<<<<< HEAD:src/Phoenix.MusiCali.Models/UserAuthZ.cs
        public string? Salt {  get; set; }
        public string? Password { get; set; }
        
=======
        public string? Salt { get; set; }
        public string? Password { get; set; }

>>>>>>> 72f693b5df2daec32f621445a51313f102d5003c:src/Latest Backend Project/TeamPhoenix.MusiCali.DataAccessLayer/Models/UserAuthZ.cs
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
<<<<<<< HEAD:src/Phoenix.MusiCali.Models/UserAuthZ.cs

        public RegistrationStatus GetRegistrationStatus()
        {
            if (isRegistrationCompleted)
            {
                return RegistrationStatus.Completed;
            }
            else
            {
                return RegistrationStatus.Incomplete;
            }
        }
        /*
        public UserAuth GetUserByUsername(string username)
        {
            Change later with DB logic
        }
        */

        public bool IsLoggedIn()
        {
            //Change Later with actual logic        
            return true;
        }

        public bool HasActiveSession()
        {
            //Change Later with actual logic
            return true;
        }

        public void SaveUser(UserAuth user)
        {
            //Change Later with DB Logic
        }
=======
        
>>>>>>> 72f693b5df2daec32f621445a51313f102d5003c:src/Latest Backend Project/TeamPhoenix.MusiCali.DataAccessLayer/Models/UserAuthZ.cs


        public bool IsAuthorize(string userIdentity, string securityContext)
        {
            //Change later with actual logic
<<<<<<< HEAD:src/Phoenix.MusiCali.Models/UserAuthZ.cs
           return true;
        }
    }

private class AuthorizationInfo{
            public string UserIdentity { get; set; }
            public string SecurityContext { get; set; }
        }
    }

        public enum UserRole
        {
            UnregisteredUser,
            RegisteredUser,
            AuthenticatedUser,
            Admin,
        }

        public enum UserPermission
        {
            InitiateAccountCreation,
            ProvideRegistrationInfo,
            VerifyEmail,
            AccessLimitedFeatures,
            AccessRegisteredUserFeatures,
            AccessAuthenticatedUserFeatures,
        }

        public enum RegistrationStatus
        {
            Incomplete,
            Completed
        }
=======
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
>>>>>>> 72f693b5df2daec32f621445a51313f102d5003c:src/Latest Backend Project/TeamPhoenix.MusiCali.DataAccessLayer/Models/UserAuthZ.cs
