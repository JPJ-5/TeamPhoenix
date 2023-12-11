using System.Security.Principal;

namespace Phoenix.MusiCali.Models
{
    public interface IMusiCaliPrincipal : IPrincipal
    {
        string Username { get; }
        UserRole Role { get; }
        // Add more properties or methods as needed
    }
}
