using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class UsageAnalysisDashboardModel //change class type if needed
    {
        private readonly string connectionString; //fix to use configuration file in the future.
        private readonly IConfiguration configuration;

        public UsageAnalysisDashboardModel(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }
        public MonthYearCountResult GetNumberOfLoginFromTimeframe(int monthsInTimeSpan, DateTime endDate)
        {
            MonthYearCountResult databaseLoginResult  = new MonthYearCountResult(null, "failed to get data", false); //default results to false.
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
                                // store the numbers of logins each month.
                                List<MonthYearCount> countPerMonths = new List<MonthYearCount>();
                                
                                for (int numberOfCurrentRow = 0; numberOfCurrentRow < allReadRows.Count; numberOfCurrentRow++)
                                {
                                    int month = (int)allReadRows[numberOfCurrentRow][1];
                                    int year = (int)allReadRows[numberOfCurrentRow][2];
                                    long count = (long)allReadRows[numberOfCurrentRow][0];
                                    countPerMonths.Add(new MonthYearCount(month, year, count));
                                }

                                databaseLoginResult = new MonthYearCountResult(countPerMonths, "Successful retrieval of login attempts", true);
                                return databaseLoginResult;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                databaseLoginResult.ErrorMessage = ex.ToString();
            }
            return databaseLoginResult;
        }
        public MonthYearCountResult GetNumberOfRegistrationFromTimeframe(int monthsInTimespan, DateTime endDate) // TODO: Fix to change to match GetNumberOfLogin function
        {
            MonthYearCountResult databaseRegistrationResult = new MonthYearCountResult(null, "failed to get data", false); //default results to false.
            DateTime startDate = endDate.AddMonths(-(monthsInTimespan));
            string databaseRegistrationSql = "SELECT COUNT(logID), MONTH(Timestamp), YEAR(Timestamp) FROM UserLogs WHERE Context = @Context AND Timestamp BETWEEN @StartDate AND @EndDate GROUP BY MONTH(Timestamp), YEAR(Timestamp) ORDER BY YEAR(Timestamp), MONTH(Timestamp)"; //Add date timeframe to sql.
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
                                // store the numbers of logins each month.
                                List<MonthYearCount> countPerMonths = new List<MonthYearCount>();

                                for (int numberOfCurrentRow = 0; numberOfCurrentRow < allReadRows.Count; numberOfCurrentRow++)
                                {
                                    int month = (int)allReadRows[numberOfCurrentRow][1];
                                    int year = (int)allReadRows[numberOfCurrentRow][2];
                                    long count = (long)allReadRows[numberOfCurrentRow][0];
                                    countPerMonths.Add(new MonthYearCount(month, year, count));
                                }

                                databaseRegistrationResult = new MonthYearCountResult(countPerMonths, "Successful retrieval of registration attempts", true);
                                return databaseRegistrationResult;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                databaseRegistrationResult.ErrorMessage = ex.ToString();
            }
            return databaseRegistrationResult;
        }
        public PageViewLengthResult GetLongestPageViewsFromTimeframe(int monthsInTimeSpan, DateTime endDate) // maybe rename to longest three pagee views for clarity.
        {
            PageViewLengthResult databasePageViewResult = new PageViewLengthResult(null, "", false);
            string databasePageViewSql = "SELECT AVG(PageLength) AS AveragePageLength, Context FROM PageLogs WHERE Timestamp BETWEEN @StartDate AND @EndDate GROUP BY Context ORDER BY AveragePageLength DESC";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    DateTime startDate = endDate.AddMonths(-(monthsInTimeSpan)); // should subtract the amount months in the timespan from date.
                    MySqlCommand command = new MySqlCommand(databasePageViewSql, connection); // used to create the command.     
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
                                List<PageViewLengthData> listOfPageViews = new List<PageViewLengthData>();
                                
                                int topViewCount = 3; // defaults to top 3 entries

                                //check if there is less than 3 entries in allReadRows. Set it to number of entries if that's the case.
                                if (allReadRows.Count < 3)
                                {
                                    topViewCount = allReadRows.Count;
                                }
                                for (var pageViewNumber = 0; pageViewNumber < topViewCount; pageViewNumber++)
                                {
                                    decimal pageLength = (decimal)allReadRows[pageViewNumber][0];
                                    string pageViewName = (string)allReadRows[pageViewNumber][1];
                                    listOfPageViews.Add(new PageViewLengthData(pageViewName, pageLength));
                                }
                                databasePageViewResult = new PageViewLengthResult(listOfPageViews, "Successful retrieval of top 3 page view length", true);
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
        public MonthYearCountResult GetNumberOfGigsCreatedFromTimeframe(int monthsInTimespan, DateTime endDate)
        {
            MonthYearCountResult GigCreatedResult = new MonthYearCountResult(null, "failed to get data", false); //default results to false.
            DateTime startDate = endDate.AddMonths(-(monthsInTimespan));
            string databaseGigsCreatedSql = "SELECT COUNT(logID), MONTH(Timestamp), YEAR(Timestamp) FROM UserLogs WHERE Context = @Context AND Timestamp BETWEEN @StartDate AND @EndDate GROUP BY MONTH(Timestamp), YEAR(Timestamp) ORDER BY YEAR(Timestamp), MONTH(Timestamp)"; //Add date timeframe to sql.
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(databaseGigsCreatedSql, connection); // used to create the command.     
                    using (command)
                    {
                        command.Parameters.AddWithValue("@Context", "Gig was successfully created");
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
                                // store the numbers of logins each month.
                                List<MonthYearCount> countPerMonths = new List<MonthYearCount>();

                                for (int numberOfCurrentRow = 0; numberOfCurrentRow < allReadRows.Count; numberOfCurrentRow++)
                                {
                                    int month = (int)allReadRows[numberOfCurrentRow][1];
                                    int year = (int)allReadRows[numberOfCurrentRow][2];
                                    long count = (long)allReadRows[numberOfCurrentRow][0];
                                    countPerMonths.Add(new MonthYearCount(month, year, count));
                                }

                                GigCreatedResult = new MonthYearCountResult(countPerMonths, "Successful retrieval of gig created", true);
                                return GigCreatedResult;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GigCreatedResult.ErrorMessage = ex.ToString(); //change this to specify what function caused this
            }
            return GigCreatedResult;
        }
        public ItemQuantityResult GetMostSoldItemsFromTimeframe(int monthsInTimeSpan, DateTime endDate) // maybe rename to longest three pagee views for clarity.
        {
            ItemQuantityResult databaseSoldItemsResult = new ItemQuantityResult(null, "", false);
            string databaseSoldItemsSql = "SELECT SKU, SUM(Quantity) AS QuantitySold FROM CraftReceipt WHERE SaleDate BETWEEN @StartDate AND @EndDate GROUP BY SKU ORDER BY QuantitySold DESC;";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    DateTime startDate = endDate.AddMonths(-(monthsInTimeSpan)); // should subtract the amount months in the timespan from date.
                    MySqlCommand command = new MySqlCommand(databaseSoldItemsSql, connection); // used to create the command.     
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
                                List<ItemQuantityData> listOfSoldItems = new List<ItemQuantityData>();

                                int topViewCount = 3; // defaults to top 3 entries

                                //check if there is less than 3 entries in allReadRows. Set it to number of entries if that's the case.
                                if (allReadRows.Count < 3)
                                {
                                    topViewCount = allReadRows.Count;
                                }
                                for (var soldItemsNumber = 0; soldItemsNumber < topViewCount; soldItemsNumber++)
                                {
                                    decimal itemQuantity = (decimal)allReadRows[soldItemsNumber][1];
                                    string itemName = (string)allReadRows[soldItemsNumber][0];
                                    listOfSoldItems.Add(new ItemQuantityData(itemName, itemQuantity));
                                }
                                databaseSoldItemsResult = new ItemQuantityResult(listOfSoldItems, "Successful retrieval of top 3 items sold", true);
                                return databaseSoldItemsResult;
                            }
                            else
                            {
                                databaseSoldItemsResult.ErrorMessage = "No items sold exist within that timeframe";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                databaseSoldItemsResult.ErrorMessage = ex.ToString(); //change this to specify what function caused this
            }
            return databaseSoldItemsResult;
        }
        public string GetUserHash(string username)
        {
            string userHash = ""; //starts empty if the hash for the user can't be found.
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string selectUserProfileSql = "SELECT UserHash FROM UserAccount WHERE Username = @Username";
                using (MySqlCommand cmd = new MySqlCommand(selectUserProfileSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userHash = new string(reader["UserHash"].ToString());
                        }
                    }

                }
            }
            return userHash;
        }
        public Result LogPageLength(string userHash, string level, string category, string context, int PageLength)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO PageLogs (UserHash, Timestamp, Level, Category, Context, PageLength) " +
                                   "VALUES (@UserHash, @Timestamp, @Level, @Category, @Context, @PageLength)";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserHash", userHash);
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Level", level);
                        cmd.Parameters.AddWithValue("@Category", category);
                        cmd.Parameters.AddWithValue("@Context", context);
                        cmd.Parameters.AddWithValue("@PageLength", PageLength);

                        cmd.ExecuteNonQuery();
                    }
                    var res = new Result();
                    res.HasError = false;
                    return res;

                }
                catch (Exception ex)
                {
                    var res = new Result();
                    res.HasError = true;
                    res.ErrorMessage = ex.Message;
                    return res;
                }
            }
        }
    }
}
