
using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using static Mysqlx.Notice.Warning.Types;

namespace TeamPhoenix.MusiCali.Services
{
    public class UsageAnalysisDashboardService
    {
        private readonly IConfiguration configuration;
        private readonly UsageAnalysisDashboardDAO usageAnalysisDashboardDAL;
        private readonly LoggerService usageAnalysisDashboardLogging;
        public UsageAnalysisDashboardService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.usageAnalysisDashboardDAL = new UsageAnalysisDashboardDAO(this.configuration);
            this.usageAnalysisDashboardLogging = new LoggerService(this.configuration);
        }
        //TODO: Should Service/Business Layer restrict the available Timespans of Months?
        public MonthYearCountResult GetLoginWithinTimeframeService(string username, int monthsInTimeSpan)
        {
            string level;
            string category;
            string context;
            string userHash;
            MonthYearCountResult DashboardLoginServiceResult = usageAnalysisDashboardDAL.GetLoginWithinTimeframe(monthsInTimeSpan);
            if (DashboardLoginServiceResult.Success)
            {
                level = "Info";
                category = "View";
                context = "Retrieved number of Login For Usage Analysis Dashboard";
                userHash = usageAnalysisDashboardDAL.GetUserHash(username);
                usageAnalysisDashboardLogging.CreateLog(userHash, level, category, context);
            }
            else
            {
                level = "Info";
                category = "View";
                context = "Did not retrieved number of Login For Usage Analysis Dashboard";
                userHash = usageAnalysisDashboardDAL.GetUserHash(username);
                usageAnalysisDashboardLogging.CreateLog(userHash, level, category, context);
            }
            return DashboardLoginServiceResult;
        }
        public MonthYearCountResult GetRegistrationWithinTimeframeService(string username, int monthsInTimeSpan)
        {
            string level;
            string category;
            string context;
            string userHash;
            MonthYearCountResult DashboardRegistrationServiceResult = usageAnalysisDashboardDAL.GetRegistrationWithinTimeframe(monthsInTimeSpan);
            if (DashboardRegistrationServiceResult.Success == true)
            {
                level = "Info";
                category = "View";
                context = "Retrieved number of Registration For Usage Analysis Dashboard";
                userHash = usageAnalysisDashboardDAL.GetUserHash(username);
                usageAnalysisDashboardLogging.CreateLog(userHash, level, category, context);
            }
            else
            {
                level = "Info";
                category = "View";
                context = "Did not retrieved number of Registration For Usage Analysis Dashboard";
                userHash = usageAnalysisDashboardDAL.GetUserHash(username);
                usageAnalysisDashboardLogging.CreateLog(userHash, level, category, context);
            }
            return DashboardRegistrationServiceResult;
        }
        public PageViewLengthResult GetLongestPageViewWithinTimeframeService(string username, int monthsInTimeSpan)
        {
            string level;
            string category;
            string context;
            string userHash;
            PageViewLengthResult DashboardPageViewServiceResult = usageAnalysisDashboardDAL.GetLongestPageViewWithinTimeframe(monthsInTimeSpan);
            if (DashboardPageViewServiceResult.Success == true)
            {
                level = "Info";
                category = "View";
                context = "Retrieved longest page view For Usage Analysis Dashboard";
                userHash = usageAnalysisDashboardDAL.GetUserHash(username);
                usageAnalysisDashboardLogging.CreateLog(userHash, level, category, context);
            }
            else
            {
                level = "Info";
                category = "View";
                context = "Did not retrieved longest page view For Usage Analysis Dashboard";
                userHash = usageAnalysisDashboardDAL.GetUserHash(username);
                usageAnalysisDashboardLogging.CreateLog(userHash, level, category, context);
            }
            return DashboardPageViewServiceResult;
        }
        public MonthYearCountResult GetGigsCreatedWithinTimeframeService(string username, int monthsInTimeSpan)
        {
            string level;
            string category;
            string context;
            string userHash;
            MonthYearCountResult DashboardGigCreatedServiceResult = usageAnalysisDashboardDAL.GetGigsCreatedWithinTimeframe(monthsInTimeSpan);
            if (DashboardGigCreatedServiceResult.Success == true)
            {
                level = "Info";
                category = "View";
                context = "Retrieved number of gigs created For Usage Analysis Dashboard";
                userHash = usageAnalysisDashboardDAL.GetUserHash(username);
                usageAnalysisDashboardLogging.CreateLog(userHash, level, category, context);
            }
            else
            {
                level = "Info";
                category = "View";
                context = "Did not retrieved number of gigs created For Usage Analysis Dashboard";
                userHash = usageAnalysisDashboardDAL.GetUserHash(username);
                usageAnalysisDashboardLogging.CreateLog(userHash, level, category, context);
            }
            return DashboardGigCreatedServiceResult;
        }
        public ItemQuantityResult GetItemsSoldWithinTimeframeService(string username, int monthsInTimeSpan)
        {
            string level;
            string category;
            string context;
            string userHash;
            ItemQuantityResult DashboardItemsSoldServiceResult = usageAnalysisDashboardDAL.GetItemsSoldWithinTimeframe(monthsInTimeSpan);
            if (DashboardItemsSoldServiceResult.Success == true)
            {
                level = "Info";
                category = "View";
                context = "Retrieved top 3 items sold For Usage Analysis Dashboard";
                userHash = usageAnalysisDashboardDAL.GetUserHash(username);
                usageAnalysisDashboardLogging.CreateLog(userHash, level, category, context);
            }
            else
            {
                level = "Info";
                category = "View";
                context = "Did not retrieved top 3 items sold For Usage Analysis Dashboard";
                userHash = usageAnalysisDashboardDAL.GetUserHash(username);
                usageAnalysisDashboardLogging.CreateLog(userHash, level, category, context);
            }
            return DashboardItemsSoldServiceResult;
        }
        public Result CreatePageLengthLogService(string username, string context, int pageLength)
        {
            Result pageLengthResult = usageAnalysisDashboardDAL.CreatePageLengthLog(username, context, pageLength);
            return pageLengthResult;
        }
    }
}
