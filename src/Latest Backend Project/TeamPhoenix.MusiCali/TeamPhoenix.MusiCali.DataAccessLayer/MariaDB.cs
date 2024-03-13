using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class MariaDB
    {
        private readonly IConfiguration _configuration;

        public MariaDB(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public bool connect()
        {
            // Replace these values with your actual database details
            string connectionString = _configuration.GetConnectionString("MariaDbConnectionString")!;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connected to the database.");
                    return true;
                    

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                    
                }
            }
            //Console.ReadLine(); // Pause console application
        }

        static void CreateTable(MySqlConnection connection)
        {
            // Example SQL query to create a new table
            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS YourTableName (
                ID INT AUTO_INCREMENT PRIMARY KEY,
                Name VARCHAR(255) NOT NULL,
                Age INT
            )";

            using (MySqlCommand command = new MySqlCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table created successfully.");
            }
        }

        public static Result CreateLog(string userHash, string level, string category, string context)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO UserLogs (UserHash, Timestamp, Level, Category, Context) " +
                                   "VALUES (@UserHash, @Timestamp, @Level, @Category, @Context)";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserHash", userHash);
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Level", level);
                        cmd.Parameters.AddWithValue("@Category", category);
                        cmd.Parameters.AddWithValue("@Context", context);

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