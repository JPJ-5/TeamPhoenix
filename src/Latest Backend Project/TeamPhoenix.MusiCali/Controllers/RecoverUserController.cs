using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rU = TeamPhoenix.MusiCali.Services.RecoverUser;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Mysqlx;
using Microsoft.IdentityModel.Tokens;
using MySqlX.XDevAPI.Common;



namespace TeamPhoenix.MusiCali.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecoverUserController : ControllerBase
    {
        [HttpPost("/api/RecoverUser")]
        public IActionResult RecoverUser([FromHeader] string userName)
        {
            if (rU.SendRecoveryEmail(userName))
            {
                var result = new Dictionary<bool, string>
                {
                    { true, "OTP successfully sent to recovery email, Secondary email has now become primary source for otp." }
                };
                return Ok(result);
            }
            var errorResult = new Dictionary<bool, string>
            {
                { false, "Unable To Recover User" }
            };
            return NotFound(errorResult);
        }

        [HttpPost("/api/DisableUser")]
        public IActionResult disableUser(string userName)
        {
            if (rU.DisableUser(userName))
            {
                var result = new Dictionary<bool, string>
                {
                    { true, "Disabled User Successfully" }
                };
                return Ok(result);
            }
            var errorResult = new Dictionary<bool, string>
            {
                { false, "Unable To Disable User" }
            };
            return NotFound(errorResult);
        }

        [HttpPost("/api/EnableUser")]
        public IActionResult enableUser(string userName)
        {
            if (rU.EnableUser(userName))
            {
                var result = new Dictionary<bool, string>
                {
                    { true, "Enabled User Successfully" }
                };
                return Ok(result);
            }
            var errorResult = new Dictionary<bool, string>
            {
                { false, "Unable To Enable User" }
            };
            return NotFound(errorResult);
        }
    }
}