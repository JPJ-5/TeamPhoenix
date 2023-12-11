using Phoenix.MusiCali.Models;
using Phoenix.MusiCali.

namespace Services;
using System.Security.Principal;
public class MusiCaliAuthService: IAuthenticator, IAuthorizer
{
    public IPrincipal Authenticate(AuthenticationRequest authRequest)
    {
        (string userIdentity, string roleName) = (null,null);
        try{
            //Step 1: Validate Auth Request

            //Step 2: Populate App Principle Object
            var roles = new Dictionary<string, string>();
            
            
        }
        catch{

        }
    }

    public bool IsAuthorize(MusiCaliPrincipal currentPrincipal, string securityContext)
    {
        throw new NotImplementedException();
    }

    
}