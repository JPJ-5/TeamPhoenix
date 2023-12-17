using System;
using System.Data.SqlClient;
public class DataAccess
{
    public void SaveToDataStore(string data)
    {
        string connectionString = "Server=tcp:julie0126.database.windows.net,1433;Initial Catalog=CraftVerify;Persist Security Info=False;User ID=Julie;Password=Jeremiah2;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Console.WriteLine("Connected to SQL Server!");

                // Insert an initial test string
                string initialTestString = "This is test number 2";
                string insertDataSql = "INSERT INTO TestMessages (Message) VALUES (@InitialTestString)";

                using (SqlCommand cmd = new SqlCommand(insertDataSql, connection))
                {
                    cmd.Parameters.AddWithValue("@InitialTestString", initialTestString);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Initial test string inserted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No rows were affected during insertion.");
                    }
                }

                connection.Close();
                Console.WriteLine("Connection closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

}