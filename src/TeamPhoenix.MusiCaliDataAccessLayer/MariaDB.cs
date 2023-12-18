using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class MariaDB
    {
        public bool connect()
        {
            // Replace these values with your actual database details
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                    Console.WriteLine("Connected to the database.");

                }
                catch (Exception ex)
                {
                    return false;
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.ReadLine(); // Pause console application
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

    }
}
