using MySql.Data.MySqlClient;
using System;
using System.Transactions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class SqlDAO : ISqlDAO
    {
        private readonly string connectionString;
        // TODO add SqlDAO that requires specific User and password
        public SqlDAO() //done for testing and default connectionString
        {
            connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
        }
        public Result ExecuteSql(string sql)
        {
            Result result = new Result(); // null result that when returns means that there was not even an attempt to connect to the database.
            MySqlTransaction transaction; // starts as an empty transaction just in case it errors before connection. This makes it so the rollback function will also not error.
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connected to the database.");

                    MySqlCommand command = new MySqlCommand(sql, connection); // used to create the command.

                    // Starts a transaction so if the write data fails, the system will rollback to the database before the writeData command.
                    transaction = connection.BeginTransaction();
                    command.Transaction = transaction; // adds the transaction to the SqlCommand object. 

                    using (command)
                    {
                        var rowsAffected = command.ExecuteNonQuery(); //used to check if anything actually changed in the table or not.
                        if (rowsAffected > 0)
                        {
                            result.Success = true;
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "No rows affected.";
                        }
                        transaction.Commit();
                        Console.WriteLine("Data successfully written.");
                    }
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback(); // will not rollback if transcaction is null.
                result = new Result(ex.Message, false);
                Console.WriteLine($"Error: {ex.Message}");
            }
            return result;
        }
    }

}