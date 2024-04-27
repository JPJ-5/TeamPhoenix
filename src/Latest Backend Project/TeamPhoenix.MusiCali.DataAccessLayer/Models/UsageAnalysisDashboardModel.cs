using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    //TODO: For all Sql lines there should be an order by date to double check.
    public class UsageAnalysisDashboardModel //change class type if needed
    {
        private readonly string connectionString; //fix to use configuration file in the future.
        private readonly IConfiguration configuration;

        public UsageAnalysisDashboardModel(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }
        public Result GetNumberOfLoginFromTimeframe(int monthsInTimeSpan, DateTime endDate)
        {
            Result databaseLoginResult = new Result("", false); //default results to false.
            DateTime startDate = endDate.AddMonths(-(monthsInTimeSpan)); // should subtract the amount months in the timespan from date.

            string databaseLoginSql = "SELECT COUNT(logID), MONTH(Timestamp), YEAR(Timestamp) FROM UserLogs WHERE Context = @Context AND Timestamp BETWEEN @StartDate AND @EndDate GROUP BY MONTH(Timestamp), YEAR(Timestamp) ORDER BY YEAR(Timestamp), MONTH(Timestamp)"; //Add date timeframe to sql.
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(databaseLoginSql, connection); // used to create the command.     
                    using (command)
                    {
                        command.Parameters.AddWithValue("@Context", "User Log In");
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);
                        using (MySqlDataReader sqlReader = command.ExecuteReader())
                        {
                            List<List<object>> allReadRows = new List<List<object>>(); //variable representing all of the rows being read with a SELECT Command.
                            while (sqlReader.Read())
                            {
                                List<object> rowRead = new List<object>();

                                for (int i = 0; i < sqlReader.FieldCount; i++) // This for loop will read every value given with a SELECT Command.
                                {
                                    rowRead.Add(sqlReader[i]); // Adds the value of an individual column on a certain row.
                                }
                                allReadRows.Add(rowRead);
                            }
                            if (allReadRows.Count > 0)
                            {
                                // calculate the numbers of logins each month.
                                Dictionary<int, int> countPerMonths = new Dictionary<int, int>();
                                for (var pageLogNumber = 0; pageLogNumber < allReadRows.Count; pageLogNumber++)
                                {
                                    DateTime dateOfLog = (DateTime)allReadRows[pageLogNumber][0];
                                    if (countPerMonths.ContainsKey(dateOfLog.Month))
                                    {
                                        countPerMonths[dateOfLog.Month]++;
                                    }
                                    else
                                    {
                                        countPerMonths.Add(dateOfLog.Month, 1);
                                    }
                                }

                                databaseLoginResult = new Result("Successful retrieval of login attempts", true);
                                databaseLoginResult.value = countPerMonths;
                                return databaseLoginResult;
                            }
                            else
                            {
                                databaseLoginResult.ErrorMessage = "No login attempts exist within that timeframe";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                databaseLoginResult.ErrorMessage = ex.ToString(); //change this to specify what function caused this
            }
            return databaseLoginResult;
        }
        public Result GetNumberOfRegistrationFromTimeframe(DateTime startDate, DateTime endDate) // TODO: Fix to change to match GetNumberOfLogin function
        {
            Result databaseRegistrationResult = new Result("", false); //default results to false.
            string databaseRegistrationSql = "SELECT COUNT(logID), MONTH(Timestamp), YEAR(Timestamp) FROM UserLogs WHERE UserContext = @Context AND Timestamp BETWEEN @StartDate AND @EndDate GROUP BY MONTH(Timestamp), YEAR(Timestamp) ORDER BY YEAR(Timestamp), MONTH(Timestamp)"; //Add date timeframe to sql.
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(databaseRegistrationSql, connection); // used to create the command.     
                    using (command)
                    {
                        command.Parameters.AddWithValue("@Context", "Creating new user");
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);
                        using (MySqlDataReader sqlReader = command.ExecuteReader())
                        {
                            List<List<object>> allReadRows = new List<List<object>>(); //variable representing all of the rows being read with a SELECT Command.
                            while (sqlReader.Read())
                            {
                                List<object> rowRead = new List<object>();

                                for (int i = 0; i < sqlReader.FieldCount; i++) // This for loop will read every value given with a SELECT Command.
                                {
                                    rowRead.Add(sqlReader[i]); // Adds the value of an individual column on a certain row.
                                }
                                allReadRows.Add(rowRead);
                            }

                            if (allReadRows.Count > 0)
                            {
                                databaseRegistrationResult = new Result("Successful retrieval of registration attempts", true);
                                databaseRegistrationResult.value = allReadRows;
                                return databaseRegistrationResult;
                            }
                            else
                            {
                                databaseRegistrationResult.ErrorMessage = "No registration attempts exist within that timeframe";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                databaseRegistrationResult.ErrorMessage = ex.ToString(); //change this to specify what function caused this
            }
            return databaseRegistrationResult;
        }
        public Result GetLongestPageViewsFromTimeframe(int monthsInTimeSpan, DateTime endDate) // maybe rename to longest three pagee views for clarity.
        {
            Result databasePageViewResult = new Result("", false);
            // TODO: change table to match the new table that has a separate log to track page length.
            string databaseRegistrationSql = "SELECT * FROM UserLogs WHERE UserContext = @Context AND Timestamp BETWEEN @StartDate AND @EndDate";
            // TODO: calc the averages of each page within timeframe.
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    DateTime startDate = endDate.AddMonths(-(monthsInTimeSpan)); // should subtract the amount months in the timespan from date.
                    MySqlCommand command = new MySqlCommand(databaseRegistrationSql, connection); // used to create the command.     
                    using (command)
                    {
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);
                        using (MySqlDataReader sqlReader = command.ExecuteReader())
                        {
                            List<List<object>> allReadRows = new List<List<object>>(); //variable representing all of the rows being read with a SELECT Command.
                            while (sqlReader.Read())
                            {
                                List<object> rowRead = new List<object>();

                                for (int i = 0; i < sqlReader.FieldCount; i++) // This for loop will read every value given with a SELECT Command.
                                {
                                    rowRead.Add(sqlReader[i]); // Adds the value of an individual column on a certain row.
                                }
                                allReadRows.Add(rowRead);
                            }

                            if (allReadRows.Count > 0)
                            {
                                // Would I do calculations in the DAL or Service layer? 
                                databasePageViewResult = new Result("Successful retrieval of page view attempts", true);
                                databasePageViewResult.value = allReadRows;
                                for (var pageLogNumber = 0; pageLogNumber < allReadRows.Count; pageLogNumber++)
                                {
                                    // allReadRows[pageLogNumber]; // still need to calc the top 3 longest page visits on average. Need to fix this still.
                                }
                                return databasePageViewResult;
                            }
                            else
                            {
                                databasePageViewResult.ErrorMessage = "No page view attempts exist within that timeframe";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                databasePageViewResult.ErrorMessage = ex.ToString(); //change this to specify what function caused this
            }
            return databasePageViewResult;
        }
    }
}
