//using TeamPhoenix.MusiCali.DataAccessLayer.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
////using TeamPhoenix.MusiCali.Security;
//using TeamPhoenix.MusiCali.Security;
//using TeamPhoenix.MusiCali.Security.Contracts;

//namespace AccCreationAPI.Controllers
//{
//    [Authorize]
//    [Route("[controller]")]
//    [ApiController]

//    public class LoginController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;
//        public LoginController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }
//        //private readonly IAuthentication _authentication;


//        //public LoginController(IAuthentication authentication)
//        //{
//        //    _authentication = authentication;
//        //}

//        [AllowAnonymous]
//        [HttpPost]
//        public async Task<IActionResult> Login([FromBody] LoginModel login)
//        {
//            //// Replace this with actual database check
//            //var userIsValid = await CheckCredentials(login.Username, login.Otp);
//            Authentication newAuth = new Authentication(_configuration);
//            bool userIsValid = newAuth.AuthenticateUsername(login.Username);
//            if (userIsValid == false)
//            {


//                throw new Exception("User Is Not Valid");
//            }
//            else
//            {
//                var token = newAuth.Authenticate(login.Username, login.Otp);
//                if(token == null)
//                {
//                    throw new Exception("Token Is Null");
//                }
//                else
//                {
//                    return Ok();
//                }
//            }

//            return BadRequest("Login Failed");
//        }

//        //private async Task<bool> CheckCredentials(string username, string otp)
//        //{
//        //    // Implement your database check here
//        //    // Return true if credentials are valid, false otherwise

//        //    return false;
//        //}

//        //[HttpGet]
//        //public IActionResult Test()
//        //{
//        //    return Ok("Token Successfully Validated");

//        //}






//    }
//}

using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Mysqlx.Session;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security;
using static System.Net.WebRequestMethods;
using aU = TeamPhoenix.MusiCali.Security.Authentication;

namespace AccCreationAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]

    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("api/GetJwtAPI")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            Authentication newAuth = new Authentication(_configuration);
            var tokens = newAuth.Authenticate(login.Username, login.Otp);

            if (tokens != null && tokens.ContainsKey("IdToken") && !string.IsNullOrEmpty(tokens["IdToken"])
                && tokens.ContainsKey("AccessToken") && !string.IsNullOrEmpty(tokens["AccessToken"]))
            {
                return Ok(tokens);
            }

            return BadRequest("Login Failed");
        }


        [AllowAnonymous]
        [HttpPost("api/CheckUsernameAPI")]
        public JsonResult CheckUsernameExist([FromBody] LoginModel login)
        {
            Authentication newAuth = new Authentication(_configuration);
            string username = login.Username;
            if (newAuth.AuthenticateUsername(username))
            {
                return new JsonResult(true);
            }
            return new JsonResult(false);

        }

        private string GenerateIdToken(string UserName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserName)
            };

            var token = new JwtSecurityToken(_configuration.GetSection("Jwt:Issuer").Value, _configuration.GetSection("Jwt:Audience").Value,
                claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private string GenerateAccessToken(string userName, IList<string> roles)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, userName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique identifier for the token
    };

            // Add role claims to the token
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30), // Token expiry time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}