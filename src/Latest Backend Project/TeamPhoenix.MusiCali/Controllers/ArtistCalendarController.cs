using Microsoft.AspNetCore.Mvc;
using artCal = TeamPhoenix.MusiCali.Services.ArtistCalendar; //artist calendar services
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
//using Microsoft.Testing.Platform.Extensions.Messages;
using TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistCalendarController : ControllerBase
    {

        [HttpPost("api/ArtistCalendarGigCreationAPI")]
        public JsonResult CreateGig([FromBody] GigCreationModel gigData)
        {
            if (artCal.createGig(gigData.Username, gigData.GigName, gigData.DateOfGig, gigData.Visibility, gigData.Location, gigData.Description, gigData.Pay))
            {
                return new JsonResult(true);
            }
            return new JsonResult(false);
        }

        [HttpPost("api/ArtistCalendarGigUpdateAPI")]
        public JsonResult UpdateGig([FromBody] GigUpdateModel gigUpdateData)
        {
            if (artCal.updateGig(gigUpdateData.DateOfGigOriginal, gigUpdateData.Username, gigUpdateData.GigName, gigUpdateData.DateOfGig, gigUpdateData.Visibility, gigUpdateData.Location, gigUpdateData.Description, gigUpdateData.Pay))
            {
                return new JsonResult(true);
            }
            return new JsonResult(false);
        }
        [HttpDelete("api/ArtistCalendarGigDeletionAPI")]
        public IActionResult DeleteGig([FromBody] GigFindModel gigDataToDelete)
        {
            if (artCal.deleteGig(gigDataToDelete.Username, gigDataToDelete.DateOfGig))
            {
                return Ok(true);
            }
            else
            {
                return BadRequest("Failed to delete user gig.");
            }
        }
        [HttpGet("api/ArtistCalendarGigViewAPI")]
        public ActionResult ViewGig([FromQuery] string username, [FromQuery] string usernameOwner, [FromQuery] DateTime dateOfGig)
        {
            var gigToView = artCal.viewGig(username, usernameOwner, dateOfGig);
            if (gigToView == null)
            {
                return NotFound("User gig not found.");
            }
            Console.WriteLine(gigToView.Location);
            return Ok(gigToView);
        }

        [HttpPost("api/ArtistCalendarGigVisibilityAPI")]
        public JsonResult UpdateGigVisibility(GigVisibilityModel gigVisibilityData)
        {
            if (artCal.updateGigVisibility(gigVisibilityData.Username, gigVisibilityData.GigVisibility))
            {
                return new JsonResult(true);
            }
            return new JsonResult(false);
        }
    }
}
