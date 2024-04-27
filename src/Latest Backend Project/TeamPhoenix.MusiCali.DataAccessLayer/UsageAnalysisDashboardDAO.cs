
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class UsageAnalysisDashboardDAO
    {
        //UsageAnalysisDashboardModel test = new UsageAnalysisDashboardModel(); // change line in the future
        public Result GetLoginWithinTimeframe(int monthsInTimeSpan)
        {
            Result databaseRegistrationResult = new Result("", false);
            DateTime todaysDate = DateTime.Now;
            //Result databaseRegistrationResult = test.GetNumberOfLoginFromTimeframe(monthsInTimeSpan, todaysDate);
            return databaseRegistrationResult;
        }
    }
}
