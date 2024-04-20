using MySql.Data.MySqlClient;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class Tester
    {
        private static string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
        public static void DeleteAllRows(string tableName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Delete all rows from the specified table
                    string deleteSql = $"DELETE FROM {tableName}";
                    using (MySqlCommand cmd = new MySqlCommand(deleteSql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    Console.WriteLine($"All rows deleted from {tableName} successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}. Retry or contact admin");
                }
            }
        }
    }
}
