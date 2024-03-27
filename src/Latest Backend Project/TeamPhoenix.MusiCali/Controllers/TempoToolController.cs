using Microsoft.AspNetCore.Mvc;
using Logger = TeamPhoenix.MusiCali.Logging.Logger;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TempoToolController : ControllerBase
    {

        [HttpPost("api/logTempoAPI")]
        public IActionResult LogTempo([FromBody] LogTempoRequest request)
        {
            // Perform any validation checks here

            bool success = Logger.LogTempo(request.UserName);
            return Ok(new { success });
        }
    }
}
