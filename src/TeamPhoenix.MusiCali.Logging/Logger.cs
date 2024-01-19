using SecurityLayer;
using System;
using System.Data.SqlClient;

namespace Phoenix.MusiCali.Logging
{
    
     public class Logger
    {

        private readonly string _connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
        private readonly DataAccessObject _dao;

        public Logger(string connectionString)
        {
            _dao = new DataAccessObject(connectionString);
        }

        public Result CreateLog(DateTime timestamp, string logLevel, string logCategory, string context)
        {
            var result = new Result();

            try
            {
                // Create a SQL connection using the provided connection string
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // SQL command to insert the log entry into the Logs table
                    var commandText = "INSERT INTO Logs (Timestamp, Level, Category, Context) VALUES (@Timestamp, @Level, @Category, @Context)";
                    using (var command = new SqlCommand(commandText, connection))
                    {
                        // Add parameters to the SQL command to prevent SQL injection
                        command.Parameters.AddWithValue("@Timestamp", timestamp);
                        command.Parameters.AddWithValue("@Level", logLevel);
                        command.Parameters.AddWithValue("@Category", logCategory);
                        command.Parameters.AddWithValue("@Context", context);

                        // Execute the command
                        command.ExecuteNonQuery();
                    }
                }

                result.HasError = false;
            }
            catch (Exception ex)
            {
                // Logging any exception occurred during logging process
                result.HasError = true;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public Result Log(string logLevel, string logCategory, string context)
        {
            return _dao.CreateLog(DateTime.UtcNow, logLevel, logCategory, context);
        }
    }
}
