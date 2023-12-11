using System.Security.Principal;
using Phoenix.MusiCali.Contracts;
namespace Phoenix.MusiCali.Models
{
    public class MusiCaliPrincipal : IMusicaliPrincipal
    {
        public MusiCaliPrincipal(string username, UserRole role)
        {
            Identity = new GenericIdentity(username);
            Role = role;
        }

        public IIdentity Identity { get; private set; }

        // Add properties or methods to represent additional user information or roles
        public bool IsInRole(string role)
        {
            // Implement logic to check if the user is in the specified role
            // You might fetch roles from a database or another storage
            // For simplicity, returning true for the role "Admin"
            return role == "Admin";
        }
    }
}
