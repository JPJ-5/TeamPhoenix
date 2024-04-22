using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class SqlArtistCalendar
    {
        private readonly string connectionString; //fix to use configuration file in the future.
        private readonly IConfiguration configuration;

        public SqlArtistCalendar(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }
        public bool executeSQL(string Sql, List<string> ParametersCategory, List<object>ParametersValue) //change return to a result-style object.
        {
            MySqlTransaction transaction; // starts as an empty transaction just in case it errors before connection. This makes it so the rollback function will also not error.
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                for (int parameterCount = 0; parameterCount < ParametersCategory.Count; parameterCount++)
                {
                    parameters.Add(ParametersCategory[parameterCount], ParametersValue[parameterCount]);
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(Sql, connection); // used to create the command.

                    // Starts a transaction so if the write data fails, the system will rollback to the database before the writeData command.
                    transaction = connection.BeginTransaction();
                    command.Transaction = transaction; // adds the transaction to the SqlCommand object.     
                    using (command)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.AddWithValue(parameter.Key, parameter.Value); // We don't know the exact type that value will be so AddWithValue has to be used.
                        }
                        var rowsAffected = command.ExecuteNonQuery(); //used to check if anything actually changed in the table or not.
                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            return true;
                        }
                        transaction.Commit(); //don't know if I need to move this line?
                    }
                }
            }
            catch (Exception ex)
            {
                //transaction?.Rollback(); // will not rollback if transcaction is null.
                return false;
            }
            return false;
        }

        public bool readSQL(string Sql, List<string> ParametersCategory, List<object> ParametersValue)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                for (int parameterCount = 0; parameterCount < ParametersCategory.Count; parameterCount++)
                {
                    parameters.Add(ParametersCategory[parameterCount], ParametersValue[parameterCount]);
                }
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(Sql, connection); // used to create the command

                    using (command)
                    {
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
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        public GigView? readGigSQL(string UsernameOfViewer, string Sql, List<string> ParametersCategory, List<object> ParametersValue)
        {
            GigView? readGigSqlResult = null;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                for (int parameterCount = 0; parameterCount < ParametersCategory.Count; parameterCount++)
                {
                    parameters.Add(ParametersCategory[parameterCount], ParametersValue[parameterCount]);
                }
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(Sql, connection); // used to create the command

                    using (command)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.AddWithValue(parameter.Key, parameter.Value); // We don't know the exact type that value will be so AddWithValue has to be used.
                        }
                        using (MySqlDataReader sqlReader = command.ExecuteReader())
                        {
                            if (sqlReader.Read())
                            {
                                if (sqlReader["PosterUsername"].ToString() != UsernameOfViewer && (bool)sqlReader["GigVisibility"] == false)
                                {
                                    string errorMessage = "Gig is not visible to user";
                                    return readGigSqlResult;
                                }
                                readGigSqlResult = new GigView(
                                sqlReader["PosterUsername"].ToString() ?? string.Empty,
                                sqlReader["GigName"].ToString() ?? string.Empty,
                                Convert.ToDateTime(sqlReader["GigDateTime"]),
                                (bool)sqlReader["GigVisibility"],
                                sqlReader["Description"].ToString() ?? string.Empty,
                                sqlReader["Location"].ToString() ?? string.Empty,
                                sqlReader["Pay"].ToString() ?? string.Empty);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                readGigSqlResult = null;
                return readGigSqlResult;
            }
            return readGigSqlResult;
        }

        public bool IsGigDateExist(string table, string username, DateTime gigDateTime) //tracks if the user has a gig during that timeframe already.
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string checkDuplicateSql = $"SELECT COUNT(*) FROM {table} WHERE PosterUsername = @PosterUsername AND GigDateTime = @GigDateTime";
                using (MySqlCommand cmd = new MySqlCommand(checkDuplicateSql, connection))
                {
                    cmd.Parameters.AddWithValue("@PosterUsername", username);
                    cmd.Parameters.AddWithValue("@GigDateTime", gigDateTime);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
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
    }
}
