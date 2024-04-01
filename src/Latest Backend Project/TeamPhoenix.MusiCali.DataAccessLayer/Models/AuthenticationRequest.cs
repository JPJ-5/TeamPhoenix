namespace Phoenix.MusiCali.Models;

public class AuthenticationRequest
{
    public string userIdentity{get;set;} = string.Empty;
    public string proof {get;set;} = string.Empty;

}