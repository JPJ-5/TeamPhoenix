using System;
using Phoenix.MusiCali.Models;

namespace Services;
public class MusiCaliAuthService: IAuthenticator, IAuthorizer
{
    public (string userIdentity, string roleName) Authenticate(AuthenticationRequest authRequest)
    {
        throw new NotImplementedException();
    }

    public bool IsAuthorize(string userIdentity, string securityContext)
    {
        throw new NotImplementedException();
    }

    
}