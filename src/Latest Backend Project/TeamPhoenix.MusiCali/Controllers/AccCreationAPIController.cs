using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using uC = TeamPhoenix.MusiCali.Services.UserCreation;


namespace AccCreationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccCreationAPIController : ControllerBase
    {

        [HttpPost("api/NormalAccCreationAPI")]
        public IActionResult RegisterNormalUser([FromBody] AccCreationModel registration)
        {
            if (uC.RegisterNormalUser(registration.Email, registration.Dob, registration.Uname, registration.Bmail))
            {
                return Ok(true); // Changed from JsonResult to IActionResult with Ok result
            }
            return BadRequest(false); // Changed from JsonResult to IActionResult with Ok result
        }

        [HttpPost("api/AdminAccCreationAPI")]
        public IActionResult RegisterAdminUser([FromBody] AccCreationModel model)
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

    }
}
