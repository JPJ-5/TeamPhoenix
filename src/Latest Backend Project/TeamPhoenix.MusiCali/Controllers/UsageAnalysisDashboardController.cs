using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsageAnalysisDashboardController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public UsageAnalysisDashboardService analysisDashboardService;

        public UsageAnalysisDashboardController(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.analysisDashboardService = new UsageAnalysisDashboardService(this.configuration);
        }

        //might need to change what is retrived from controller here.
        [HttpPost("api/UsageAnalysisDashboardGetLoginAPI")]
        public IActionResult GetLoginWithinTimeframe([FromQuery] string username, [FromQuery] int monthsInTimeSpan)
        {
            Result loginsToView = analysisDashboardService.GetLoginWithinTimeframeService(username, monthsInTimeSpan);
            if (loginsToView.Success)
            {
                Ok(loginsToView);
            }
            return BadRequest(loginsToView.ErrorMessage);
        }
        [HttpPost("api/UsageAnalysisDashboardGetRegistrationAPI")]
        public IActionResult GetRegistrationWithinTimeframe([FromQuery] string username, [FromQuery] int monthsInTimeSpan)
        {
            Result RegistrationToView = analysisDashboardService.GetRegistrationWithinTimeframeService(username, monthsInTimeSpan);
            if (RegistrationToView.Success)
            {
                Ok(RegistrationToView);
            }
            return BadRequest(RegistrationToView.ErrorMessage);
        }
        [HttpPost("api/UsageAnalysisDashboardGetLongestPageViewAPI")]
        public IActionResult GetLongestPageViewWithinTimeframe([FromQuery] string username, [FromQuery] int monthsInTimeSpan)
        {
            Result LongestPageViewToView = analysisDashboardService.GetLongestPageViewWithinTimeframeService(username, monthsInTimeSpan);
            if (LongestPageViewToView.Success)
            {
                Ok(LongestPageViewToView);
            }
            return BadRequest(LongestPageViewToView.ErrorMessage);
        }
        [HttpPost("api/UsageAnalysisDashboardGetGigsCreatedAPI")]
        public IActionResult GetGigsCreatedWithinTimeframe([FromQuery] string username, [FromQuery] int monthsInTimeSpan)
        {
            Result GigsCreatedToView = analysisDashboardService.GetGigsCreatedWithinTimeframeService(username, monthsInTimeSpan);
            if (GigsCreatedToView.Success)
            {
                Ok(GigsCreatedToView);
            }
            return BadRequest(GigsCreatedToView.ErrorMessage);
        }

    }
}
