using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecoverUserController : ControllerBase
    {
        private readonly RecoverUserService recoverUserService;
        private readonly IConfiguration configuration;

        public RecoverUserController(IConfiguration configuration)
        {
            this.configuration = configuration;
            recoverUserService = new RecoverUserService(configuration);
        }

        [HttpPost("/api/RecoverUser")]
        public IActionResult RecoverUser([FromHeader] string userName)
        {
            if (recoverUserService.SendRecoveryEmail(userName))
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
        public IActionResult DisableUser(string userName)
        {
            if (recoverUserService.DisableUser(userName))
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
        public IActionResult EnableUser(string userName)
        {
            if (recoverUserService.EnableUser(userName))
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