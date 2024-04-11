using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using BB = TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.Services.BingoBoard;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BingoBoardController : Controller
    {
        [HttpGet("api/BingoBoardLoadGigs")]
        public ActionResult ViewMultipleGigs([FromQuery] ushort numberOfGigs, [FromQuery] string username)
        {
            var gigSummaries = BB.ViewMultGigSummary(numberOfGigs, username);
            if (gigSummaries == null)
            {
                return NotFound("Error retrieving gigs");
            }
            return Ok(gigSummaries);
        }
    }
}
