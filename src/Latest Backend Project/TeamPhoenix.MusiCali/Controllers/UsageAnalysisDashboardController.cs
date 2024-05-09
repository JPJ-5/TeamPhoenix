using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsageAnalysisDashboardController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly UsageAnalysisDashboardService analysisDashboardService;

        public UsageAnalysisDashboardController(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.analysisDashboardService = new UsageAnalysisDashboardService(this.configuration);
        }

        //might need to change what is retrived from controller here.
        [HttpGet("api/UsageAnalysisDashboardGetLoginAPI")]
        public IActionResult GetLoginWithinTimeframe([FromQuery] string username, [FromQuery] int monthsInTimeSpan)
        {
            MonthYearCountResult loginsToView = analysisDashboardService.GetLoginWithinTimeframeService(username, monthsInTimeSpan);
            if (loginsToView.Success)
            {
                List<DateTime> monthsInResult = [];
                List<long> countsInResult = [];
                for (int i = 0; i < loginsToView.Values?.Count; i++)
                {
                    MonthYearCount item = loginsToView.Values[i];
                    monthsInResult.Add(item.monthYear);
                    countsInResult.Add(item.count);
                }
                return Ok(new
                {
                    months = monthsInResult,
                    count = countsInResult
                });
            }
            return BadRequest(loginsToView.ErrorMessage);
        }
        [HttpGet("api/UsageAnalysisDashboardGetRegistrationAPI")]
        public IActionResult GetRegistrationWithinTimeframe([FromQuery] string username, [FromQuery] int monthsInTimeSpan)
        {
            MonthYearCountResult registrationToView = analysisDashboardService.GetRegistrationWithinTimeframeService(username, monthsInTimeSpan);
            if (registrationToView.Success)
            {
                List<DateTime> monthsInResult = [];
                List<long> countsInResult = [];
                for (int i = 0; i < registrationToView.Values?.Count; i++)
                {
                    MonthYearCount item = registrationToView.Values[i];
                    monthsInResult.Add(item.monthYear);
                    countsInResult.Add(item.count);
                }
                return Ok(new
                {
                    months = monthsInResult,
                    count = countsInResult
                });
            }
            return BadRequest(registrationToView.ErrorMessage);
        }
        [HttpGet("api/UsageAnalysisDashboardGetLongestPageViewAPI")]
        public IActionResult GetLongestPageViewWithinTimeframe([FromQuery] string username, [FromQuery] int monthsInTimeSpan)
        {
            PageViewLengthResult longestPageViewToView = analysisDashboardService.GetLongestPageViewWithinTimeframeService(username, monthsInTimeSpan);
            if (longestPageViewToView.Success)
            {
                List<string> pageNamesInResult = [];
                List<decimal> lengthsInResult = [];
                for (int i = 0; i < longestPageViewToView.Values?.Count; i++)
                {
                    PageViewLengthData item = longestPageViewToView.Values[i];
                    pageNamesInResult.Add(item.pageViewName);
                    lengthsInResult.Add(item.length);
                }
                return Ok(new
                {
                    pageNames = pageNamesInResult,
                    lengthOfPage = lengthsInResult
                });
            }
            return BadRequest(longestPageViewToView.ErrorMessage);
        }
        [HttpGet("api/UsageAnalysisDashboardGetGigsCreatedAPI")]
        public IActionResult GetGigsCreatedWithinTimeframe([FromQuery] string username, [FromQuery] int monthsInTimeSpan)
        {
            MonthYearCountResult gigsCreatedToView = analysisDashboardService.GetGigsCreatedWithinTimeframeService(username, monthsInTimeSpan);
            if (gigsCreatedToView.Success)
            {
                List<DateTime> monthsInResult = [];
                List<long> countsInResult = [];
                for (int i = 0; i < gigsCreatedToView.Values?.Count; i++)
                {
                    MonthYearCount item = gigsCreatedToView.Values[i];
                    monthsInResult.Add(item.monthYear);
                    countsInResult.Add(item.count);
                }
                return Ok(new
                {
                    months = monthsInResult,
                    count = countsInResult
                });
            }
            return BadRequest(gigsCreatedToView.ErrorMessage);
        }
        [HttpGet("api/UsageAnalysisDashboardGetItemsSoldAPI")]
        public IActionResult GetItemsSoldWithinTimeframe([FromQuery] string username, [FromQuery] int monthsInTimeSpan)
        {
            ItemQuantityResult ItemsSoldToView = analysisDashboardService.GetItemsSoldWithinTimeframeService(username, monthsInTimeSpan);
            if (ItemsSoldToView.Success)
            {
                List<string> itemNamesInResult = [];
                List<decimal> quantitiesInResult = [];
                for (int i = 0; i < ItemsSoldToView.Values?.Count; i++)
                {
                    ItemQuantityData item = ItemsSoldToView.Values[i];
                    itemNamesInResult.Add(item.itemName);
                    quantitiesInResult.Add(item.quantity);
                }
                return Ok(new
                {
                    itemNames = itemNamesInResult,
                    quantity = quantitiesInResult
                });
            }
            return BadRequest(ItemsSoldToView.ErrorMessage);
        }
        [HttpPost("api/UsageAnalysisDashboardLogPageLengthAPI")]
        public IActionResult CreatePageLengthLog([FromBody] PageViewLengthLoggingRequest pageLengthData)
        {
            Result pageLengthResult = analysisDashboardService.CreatePageLengthLogService(pageLengthData.Username, pageLengthData.Context, pageLengthData.PageLength);

            if (pageLengthResult.HasError == false)
            {
                return Ok(pageLengthResult);
            }
            return BadRequest(pageLengthResult.ErrorMessage);
        }
    }
}
