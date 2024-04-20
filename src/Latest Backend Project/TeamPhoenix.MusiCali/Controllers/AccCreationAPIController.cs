using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.Security;


namespace AccCreationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccCreationAPIController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private AuthenticationSecurity authenticationSecurity;

        public AccCreationAPIController(IConfiguration configuration)
        {
            this.configuration = configuration;
            authenticationSecurity = new AuthenticationSecurity(configuration);
        }

        [HttpPost("api/NormalAccCreationAPI")]
        public IActionResult RegisterNormalUser([FromBody] AccCreationModel registration)
        {
            UserCreationService uC = new UserCreationService(configuration);
            if (uC.RegisterNormalUser(registration.Email, registration.Dob, registration.Uname, registration.Bmail))
            {
                return Ok(true); // Changed from JsonResult to IActionResult with Ok result
            }
            return BadRequest(false); // Changed from JsonResult to IActionResult with Ok result
        }

        [HttpPost("api/AdminAccCreationAPI")]
        public IActionResult RegisterAdminUser([FromBody] AccCreationModel model)
        {
            UserCreationService uC = new UserCreationService(configuration);
            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            //Console.WriteLine(accessToken);


            var role = authenticationSecurity.getScopeFromToken(accessToken!);

            var user = authenticationSecurity.getUserFromToken(accessToken!);


            if ((role != string.Empty) && authenticationSecurity.CheckIdRoleExisting(user, role))
            {

                if (model == null)
                {
                    return BadRequest("Invalid data"); // Changed from JsonResult to IActionResult with BadRequest result
                }

                if (uC.RegisterAdminUser(model.Email, model.Dob, model.Uname, model.Bmail))
                {
                    return Ok(true); // Changed from JsonResult to IActionResult with Ok result
                }
                return BadRequest(false); // Changed from JsonResult to IActionResult with Ok result

            }
            else
            {
                return BadRequest("Unauthenticated!");
            }

            
        }

    }
}
