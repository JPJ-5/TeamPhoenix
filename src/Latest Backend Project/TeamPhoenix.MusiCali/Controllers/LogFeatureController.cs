using Microsoft.AspNetCore.Mvc;
using Logger = TeamPhoenix.MusiCali.Logging.Logger;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogFeatureController : ControllerBase
    {

        [HttpPost("api/LogFeatureAPI")]
        public IActionResult LogFeature([FromBody] LogFeature request)
        {
            // Perform any validation checks here

            Result success = Logger.LogFeature(request.UserName, request.Feature);
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