using Microsoft.AspNetCore.Mvc;
using static TeamPhoenix.MusiCali.Services.ArtistCalendarService; //fix this in the future to be implemented as a project reference instead.
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistCalendarController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public ArtistCalendarService artistCalendarService;
        public ArtistCalendarController(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.artistCalendarService = new ArtistCalendarService(this.configuration);
        }
        [HttpPost("api/ArtistCalendarGigCreationAPI")]
        public IActionResult CreateGig([FromBody] GigCreationModel gigData)
        {
            Result gigResult = artistCalendarService.CreateGigService(gigData.Username, gigData.GigName, gigData.DateOfGig, gigData.Visibility, gigData.Location, gigData.Description, gigData.Pay);
            if (gigResult.Success)
            {
                return Ok(gigResult); //change this result probably
            }
            return BadRequest(gigResult.ErrorMessage);
        }

        [HttpPost("api/ArtistCalendarGigUpdateAPI")]
        public IActionResult UpdateGig([FromBody] GigUpdateModel gigUpdateData)
        {
            Result gigResult = artistCalendarService.UpdateGigService(gigUpdateData.DateOfGigOriginal, gigUpdateData.Username, gigUpdateData.GigName, gigUpdateData.DateOfGig, gigUpdateData.Visibility, gigUpdateData.Location, gigUpdateData.Description, gigUpdateData.Pay);
            if (gigResult.Success)
            {
                return Ok(gigResult); //change this result probably
            }
            return BadRequest(gigResult.ErrorMessage);
        }
        [HttpDelete("api/ArtistCalendarGigDeletionAPI")]
        public IActionResult DeleteGig([FromBody] GigFindModel gigDataToDelete)
        {
            Result gigResult = artistCalendarService.DeleteGigService(gigDataToDelete.Username, gigDataToDelete.DateOfGig);
            if (gigResult.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(gigResult.ErrorMessage);
            }
        }
        [HttpGet("api/ArtistCalendarGigViewAPI")]
        public IActionResult ViewGig([FromQuery] string username, [FromQuery] string usernameOwner, [FromQuery] DateTime dateOfGig)
        {
            var gigToView = artistCalendarService.ViewGigService(username, usernameOwner, dateOfGig);
            if (gigToView == null)
            {
                return NotFound("User gig not found.");
            }
            return Ok(gigToView);
        }

        [HttpPost("api/ArtistCalendarGigVisibilityAPI")]
        public IActionResult UpdateGigVisibility(GigVisibilityModel gigVisibilityData)
        {
            Result gigResult = artistCalendarService.UpdateGigVisibilityService(gigVisibilityData.Username, gigVisibilityData.GigVisibility);
            if (gigResult.Success)
            {
                return Ok(gigResult); //change this value.
            }
            return BadRequest(gigResult.ErrorMessage);
        }
    }
}
