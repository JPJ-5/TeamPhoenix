namespace Phoenix.MusiCali.Models;
public interface IAuthenticator
    {
        //ValueTuple
        (string userIdentity, string roleName) Authenticate(AuthenticationRequest authRequest);
    }