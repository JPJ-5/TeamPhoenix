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
        public IActionResult RegisterNormalUser(string email, DateTime dob, string uname, string bmail)
        {
            if (uC.RegisterNormalUser(email, dob, uname, bmail))
            {
                return Ok(true); // Changed from JsonResult to IActionResult with Ok result
            }
            return Ok(false); // Changed from JsonResult to IActionResult with Ok result
        }

        public class AdminUserModel
        {
            public string Email { get; set; } = string.Empty;
            public DateTime Dob { get; set; }
            public string Uname { get; set; } = string.Empty;   
            public string Bmail { get; set; } = string.Empty;
        }

        [HttpPost("api/AdminAccCreationAPI")]
        public IActionResult RegisterAdminUser([FromBody] AdminUserModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data"); // Changed from JsonResult to IActionResult with BadRequest result
            }

            if (uC.RegisterAdminUser(model.Email, model.Dob, model.Uname, model.Bmail))
            {
                return Ok(true); // Changed from JsonResult to IActionResult with Ok result
            }
            return Ok(false); // Changed from JsonResult to IActionResult with Ok result
        }

    }
}
