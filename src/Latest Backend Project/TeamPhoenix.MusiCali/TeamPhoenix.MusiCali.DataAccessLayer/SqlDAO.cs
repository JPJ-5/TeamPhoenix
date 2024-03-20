using MySql.Data.MySqlClient;
using System;
using System.Reflection.PortableExecutable;
using TeamPhoenix.MusiCali.DataAccessLayer.Contract;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class SqlDAO : ISqlDAO
    {
        private readonly string connectionString;
        public SqlDAO() //done for testing and default connectionString
        {
            connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
        }
        public SqlDAO(string username, string password)
        {
            connectionString = string.Format(@"Server=3.142.241.151;Database=MusiCali;User ID={0};Password={1};", username, password);
        }
        public Result ExecuteSql(string sql)
        {
            Result result = new Result(); // null result when returns means that there was not even an attempt to connect to the database.
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
                result = new Result(ex.Message, false);
                Console.WriteLine($"Error: {ex.Message}");
                //transaction?.Rollback(); // will not rollback if transcaction is null.
            }
            return result;
        }
        public Result ExecuteSql(string sql, Dictionary<string, object> parameters) //used with sql strings with parameters.
        {
            Result result = new Result(); // null result when returns means that there was not even an attempt to connect to the database.
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
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.AddWithValue(parameter.Key, parameter.Value); // We don't know the exact type that value will be so AddWithValue has to be used.
                        }
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
                result = new Result(ex.Message, false);
                Console.WriteLine($"Error: {ex.Message}");
                //transaction?.Rollback(); // will not rollback if transcaction is null.
            }
            return result;
        }
        public Result ReadSql(string sql)
        {
            Result result = new Result(); // null result when returns means that there was not even an attempt to connect to the database.
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connected to the database.");

                    MySqlCommand command = new MySqlCommand(sql, connection); // used to create the command

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
                                result.Success = true;
                                result.value = allReadRows;
                            }
                            else
                            {
                                result.Success = false;
                                result.ErrorMessage = "No rows found.";
                            }
                        }
                        Console.WriteLine("Data successfully read.");
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Result(ex.Message, false);
                Console.WriteLine($"Error: {ex.Message}");
            }
            return result;
        }
    }
}