using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class UserDeletion
    {
        // Hardcoded connection string
        private static string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

        public static bool DeleteProfile(string username)
        {
            string queryUserProfile = "DELETE FROM UserProfile WHERE Username = @Username";
            string queryUserAccount = "DELETE FROM UserAccount WHERE Username = @Username";


            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        // Attempt to delete from UserProfile
                        using (var commandUserProfile = new MySqlCommand(queryUserProfile, connection, transaction))
                        {
                            commandUserProfile.Parameters.AddWithValue("@Username", username);
                            int userProfileResult = commandUserProfile.ExecuteNonQuery(); // Execute and check result

                            // Optionally, check the result of the execution to ensure that a record was deleted
                            // Similar check can be done for UserAccount if needed
                        }

                        // Attempt to delete from UserAccount
                        using (var commandUserAccount = new MySqlCommand(queryUserAccount, connection, transaction))
                        {
                            commandUserAccount.Parameters.AddWithValue("@Username", username);
                            int userAccountResult = commandUserAccount.ExecuteNonQuery(); // Execute and check result
                        }

                        // Commit transaction assuming no exception occurred
                        transaction.Commit();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                // If an exception occurs, it means there was a problem with the connection, command, or transaction
                // Not necessarily with the existence of the user records
                return false;
            }
        }

        public static string GetUserHash(string username)
        {
            // Implementation remains the same for GetUserHash
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
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
                            string userHash = reader["UserHash"].ToString()!;
                            return userHash;
                        }
                    }
                }
            }
            return string.Empty; // Return null if user hash is not found
        }
    }
}