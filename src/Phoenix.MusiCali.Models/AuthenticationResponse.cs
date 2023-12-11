namespace Phoenix.MusiCali.Models;

public class AuthenticationResponse
{
    //public string hasError=>Principal is null? true; false;
    public string canRetry{get;set;} = string.Empty;

    public MusiCaliPrincipal? Principal {get;set;}

    //Identity: Who you are

    //Principle: Who you are in context
}