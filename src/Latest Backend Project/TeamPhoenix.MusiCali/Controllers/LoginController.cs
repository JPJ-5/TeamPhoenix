using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Mysqlx.Session;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        public IActionResult CheckUsernameExist([FromBody] LoginModel login)
        {
            Authentication newAuth = new Authentication(_configuration);
            string username = login.Username;
            if (newAuth.AuthenticateUsername(username))
            {
                return Ok(true); // Changed from JsonResult to IActionResult with Ok result
            }
            return Ok(false); // Changed from JsonResult to IActionResult with Ok result
        }

        private string GenerateIdToken(string UserName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));

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
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
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