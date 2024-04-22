using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Logging;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogFeatureController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private LoggerService loggerService;
        public LogFeatureController(IConfiguration configuration)
        {
            this.configuration = configuration;
            loggerService = new LoggerService(configuration);
        }   

        [HttpPost("api/LogFeatureAPI")]
        public IActionResult LogFeature([FromBody] LogFeature request)
        {
            // Perform any validation checks here

            Result success = loggerService.LogFeature(request.UserName, request.Feature);
            if (!success.HasError)
            {
                return Ok(new { success });
            }
            else
            {
                return BadRequest(new { error = "Unable to Log Feature Usage" });
            }
        }
    }
}