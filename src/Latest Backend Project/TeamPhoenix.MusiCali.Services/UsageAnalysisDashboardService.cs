
using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;

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
        public Result GetLoginWithinTimeframeService(string username, int monthsInTimeSpan)
        {
            string level;
            string category;
            string context;
            string userHash;
            Result DashboardLoginServiceResult = usageAnalysisDashboardDAL.GetLoginWithinTimeframe(monthsInTimeSpan);
            if (DashboardLoginServiceResult.Success == true)
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
        public Result GetRegistrationWithinTimeframeService(string username, int monthsInTimeSpan)
        {
            string level;
            string category;
            string context;
            string userHash;
            Result DashboardRegistrationServiceResult = usageAnalysisDashboardDAL.GetRegistrationWithinTimeframe(monthsInTimeSpan);
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
        public Result GetLongestPageViewWithinTimeframeService(string username, int monthsInTimeSpan)
        {
            string level;
            string category;
            string context;
            string userHash;
            Result DashboardPageViewServiceResult = usageAnalysisDashboardDAL.GetLongestPageViewWithinTimeframe(monthsInTimeSpan);
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
        public Result GetGigsCreatedWithinTimeframeService(string username, int monthsInTimeSpan)
        {
            string level;
            string category;
            string context;
            string userHash;
            Result DashboardGigCreatedServiceResult = usageAnalysisDashboardDAL.GetGigsCreatedWithinTimeframe(monthsInTimeSpan);
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
    }
}
