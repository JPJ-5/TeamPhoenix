using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security;
using Microsoft.Extensions.Configuration;

namespace TeamPhoenix.MusiCali.Login
{
    public class UserLogin
    {
        public static LoginTokens AppLogin(LoginModel login, IConfiguration? _configuration)
        {
            AuthenticationSecurity newAuth = new AuthenticationSecurity(_configuration!);
            var checkExistence = newAuth.Authenticate(login.Username, login.Otp);


            if (checkExistence)
            {
                var idToken = newAuth.CreateIDJwt(login);
                var accessToken = newAuth.CreateAccessJwt(login);
                LoginTokens tokens = new LoginTokens
                {
                    IdToken = idToken,
                    AccToken = accessToken,
                    Success = true
                };

                return tokens;
            }
            else
            {
                LoginTokens tokens = new LoginTokens { Success = false };
                return tokens;
            }
        }
    }
}