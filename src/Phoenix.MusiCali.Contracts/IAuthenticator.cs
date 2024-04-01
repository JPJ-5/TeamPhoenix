namespace Phoenix.MusiCali.Contracts;
public interface IAuthorizer
    {
        bool IsAuthorize(string userIdentity, string securityContext);
    }