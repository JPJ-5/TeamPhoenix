using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security;
using Microsoft.Extensions.Configuration;

namespace TeamPhoenix.MusiCali.Login
{
    public class UserLogin
    {
        private static IConfiguration? _configuration;

        public UserLogin(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static LoginTokens AppLogin(LoginModel login)
        {

            Authentication newAuth = new Authentication(_configuration!);
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