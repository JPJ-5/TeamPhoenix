
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class UsageAnalysisDashboardDAO
    {
        private readonly IConfiguration configuration;
        private readonly UsageAnalysisDashboardModel usageAnalysisDatabaseAccess;

        public UsageAnalysisDashboardDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.usageAnalysisDatabaseAccess = new UsageAnalysisDashboardModel(this.configuration);
        }
        public MonthYearCountResult GetLoginWithinTimeframe(int monthsInTimeSpan)
        {
            DateTime todaysDate = DateTime.Now;
            MonthYearCountResult databaseLoginResult = usageAnalysisDatabaseAccess.GetNumberOfLoginFromTimeframe(monthsInTimeSpan, todaysDate);
            return databaseLoginResult;
        }
        public MonthYearCountResult GetRegistrationWithinTimeframe(int monthsInTimeSpan)
        {
            DateTime todaysDate = DateTime.Now;
            MonthYearCountResult databaseRegistrationResult = usageAnalysisDatabaseAccess.GetNumberOfRegistrationFromTimeframe(monthsInTimeSpan, todaysDate);
            return databaseRegistrationResult;
        }
        public PageViewLengthResult GetLongestPageViewWithinTimeframe(int monthsInTimeSpan)
        {
            DateTime todaysDate = DateTime.Now;
            PageViewLengthResult databasePageViewResult = usageAnalysisDatabaseAccess.GetLongestPageViewsFromTimeframe(monthsInTimeSpan, todaysDate);
            return databasePageViewResult;
        }
        public MonthYearCountResult GetGigsCreatedWithinTimeframe(int monthsInTimeSpan)
        {
            DateTime todaysDate = DateTime.Now;
            MonthYearCountResult databasePageViewResult = usageAnalysisDatabaseAccess.GetNumberOfGigsCreatedFromTimeframe(monthsInTimeSpan, todaysDate);
            return databasePageViewResult;
        }
        public ItemQuantityResult GetItemsSoldWithinTimeframe(int monthsInTimeSpan)
        {
            DateTime todaysDate = DateTime.Now;
            ItemQuantityResult databaseItemsSoldResult = usageAnalysisDatabaseAccess.GetMostSoldItemsFromTimeframe(monthsInTimeSpan, todaysDate);
            return databaseItemsSoldResult;
        }
        public string GetUserHash(string username)
        {
            string userHash = usageAnalysisDatabaseAccess.GetUserHash(username);
            return userHash;
        }
        public Result CreatePageLengthLog(string username, string context, int pageLength)
        {
            string level = "Info";
            string category = "View";
            string userHash = GetUserHash(username);
            Result pageLengthResult = usageAnalysisDatabaseAccess.LogPageLength(userHash, level, category, context, pageLength);
            return pageLengthResult;
        }
    }
}
