using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class BingoBoardController : Controller
    {

        private readonly IConfiguration configuration;
        private BingoBoardService bingoBoardService;
        public BingoBoardController(IConfiguration configuration)
        {
            this.configuration = configuration;
            bingoBoardService = new BingoBoardService(configuration);

        }
        [HttpPost("api/BingoBoardLoadGigs")]
        public ActionResult ViewMultipleGigs([FromBody] BingoBoardRequest BBReq)
        {
            GigSet? gigSummaries = bingoBoardService.ViewMultGigSummary(BBReq.NumberOfGigs, BBReq.Username, BBReq.Offset);
            if (gigSummaries == null)
            {
                return NotFound("Error retrieving gigs");
            }
            return Ok(gigSummaries);
        }

        [HttpGet("api/BingoBoardRetrieveGigTableSize")]
        public ActionResult RetrieveGigTableSize()
        {
            int gigTableSize = bingoBoardService.ReturnGigNum();
            if(gigTableSize <= 0) { return NotFound("Error retrieving Gig Table size"); }
            return Ok(gigTableSize);
        }

        [HttpPost("api/BingoBoardInterestRequest")]
        public ActionResult IsUserInterested([FromBody] BingoBoardInterestRequest BBIntReq)
        {
            bool userInterest = bingoBoardService.IsUserInterested(BBIntReq.username, BBIntReq.gigID);
            return Ok(userInterest);
        }

        [HttpPost("api/BingoBoardRegisterUserInterest")]
        public ActionResult RegisterUserInterest([FromBody] BingoBoardInterestRequest BBIntReq)
        {
            BingoBoardInterestMessage intMessage = bingoBoardService.addUserInterest(BBIntReq.username, BBIntReq.gigID);
            return Ok(intMessage);
        }
    }
}