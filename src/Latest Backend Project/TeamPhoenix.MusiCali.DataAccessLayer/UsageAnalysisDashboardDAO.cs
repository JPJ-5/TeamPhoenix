
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
        public Result GetLoginWithinTimeframe(int monthsInTimeSpan)
        {
            DateTime todaysDate = DateTime.Now;
            Result databaseLoginResult = usageAnalysisDatabaseAccess.GetNumberOfLoginFromTimeframe(monthsInTimeSpan, todaysDate);
            return databaseLoginResult;
        }
        public Result GetRegistrationWithinTimeframe(int monthsInTimeSpan)
        {
            DateTime todaysDate = DateTime.Now;
            Result databaseRegistrationResult = usageAnalysisDatabaseAccess.GetNumberOfRegistrationFromTimeframe(monthsInTimeSpan, todaysDate);
            return databaseRegistrationResult;
        }
        public Result GetLongestPageViewWithinTimeframe(int monthsInTimeSpan)
        {
            DateTime todaysDate = DateTime.Now;
            Result databasePageViewResult = usageAnalysisDatabaseAccess.GetLongestPageViewsFromTimeframe(monthsInTimeSpan, todaysDate);
            return databasePageViewResult;
        }
        public Result GetGigsCreatedWithinTimeframe(int monthsInTimeSpan)
        {
            DateTime todaysDate = DateTime.Now;
            Result databasePageViewResult = usageAnalysisDatabaseAccess.GetNumberOfGigsCreatedFromTimeframe(monthsInTimeSpan, todaysDate);
            return databasePageViewResult;
        }

        public string GetUserHash(string username)
        {
            string userHash = usageAnalysisDatabaseAccess.GetUserHash(username);
            return userHash;
        }

    }
}
