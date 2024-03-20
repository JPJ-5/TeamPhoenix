using Microsoft.AspNetCore.Authorization;
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
        public JsonResult RegisterNormalUser(string email, DateTime dob, string uname, string bmail)
        {
            if (uC.RegisterNormalUser(email, dob, uname, bmail))
            {
                return new JsonResult(true);
            }
            return new JsonResult(false);

        }

        public class AdminUserModel
        {
            public string Email { get; set; } = string.Empty;
            public DateTime Dob { get; set; }
            public string Uname { get; set; } = string.Empty;   
            public string Bmail { get; set; } = string.Empty;
        }

        [Authorize(Roles = "AdminUser")]
        [HttpPost("api/AdminAccCreationAPI")]
        public JsonResult RegisterAdminUser([FromBody] AdminUserModel model)
        {
            if (model == null)
            {
                return new JsonResult("Invalid data") { StatusCode = 400 }; // Bad Request
            }

            if (uC.RegisterAdminUser(model.Email, model.Dob, model.Uname, model.Bmail))
            {
                return new JsonResult(true);
            }
            return new JsonResult(false);
        }

    }
}
