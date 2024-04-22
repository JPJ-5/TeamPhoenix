using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using TeamPhoenix.MusiCali.Security;
using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Login;
using Newtonsoft.Json.Linq;

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
           
            var tokens = UserLogin.AppLogin(login, _configuration);
            if (tokens.Success == true)
            {
                Dictionary<string, string> Tokens = new Dictionary<string, string>();
                Tokens["IdToken"] = tokens.IdToken!;
                Tokens["AccessToken"] = tokens.AccToken!;
                return Ok(Tokens);
            } else
            {
                return BadRequest("Login Failed");
            }

        }


        [AllowAnonymous]
        [HttpPost("api/CheckUsernameAPI")]
        public IActionResult CheckUsernameExist([FromBody] LoginModel login)
        {
            AuthenticationSecurity newAuth = new AuthenticationSecurity(_configuration);
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