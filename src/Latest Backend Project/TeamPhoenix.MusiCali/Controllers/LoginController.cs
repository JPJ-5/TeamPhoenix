using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Security;

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
        public IActionResult Login([FromBody] LoginModel login)
        {
            Authentication newAuth = new Authentication(_configuration);
            var checkExistence = newAuth.Authenticate(login.Username, login.Otp);

            //if (tokens != null && tokens.ContainsKey("IdToken") && !string.IsNullOrEmpty(tokens["IdToken"])
            //    && tokens.ContainsKey("AccessToken") && !string.IsNullOrEmpty(tokens["AccessToken"]))
            //{
            //    return Ok(tokens);
            //}
            if (checkExistence)
            {
                var idToken = newAuth.CreateIDJwt(login);
                var accessToken = newAuth.CreateAccessJwt(login);
                Dictionary<string, string> tokens = new Dictionary<string, string>();
                tokens["IdToken"] = idToken;
                tokens["AccessToken"] = accessToken;
                //Console.WriteLine(tokens);
                return Ok(tokens);
            }

            return BadRequest("Login Failed");
        }


        [AllowAnonymous]
        [HttpPost("api/CheckUsernameAPI")]
        public IActionResult CheckUsernameExist([FromBody] LoginModel login)
        {
            Authentication newAuth = new Authentication(_configuration);
            string username = login.Username;
            if (newAuth.AuthenticateUsername(username))
            {
                return Ok(true); // Changed from JsonResult to IActionResult with Ok result
            }
            return BadRequest(false); // Changed from JsonResult to IActionResult with Ok result
        }

        //[AllowAnonymous]
        //[HttpPost("/secure/createIDToken")]
    }  
}