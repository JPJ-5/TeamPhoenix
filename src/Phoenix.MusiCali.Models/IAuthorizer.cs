namespace Phoenix.MusiCali.Models;
public interface IAuthorizer
    {
        bool IsAuthorize(string userIdentity, string securityContext);
    }