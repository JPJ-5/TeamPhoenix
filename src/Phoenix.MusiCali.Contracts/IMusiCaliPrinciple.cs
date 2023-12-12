using System.Security.Principal;

namespace Phoenix.MusiCali.Contracts
{
    public interface IMusiCaliPrincipal : IPrincipal
    {
        string Username { get; }
        UserRole Role { get; }
        // Add more properties or methods as needed
    }
}
