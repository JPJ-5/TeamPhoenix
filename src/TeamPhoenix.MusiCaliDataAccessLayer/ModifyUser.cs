using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class ModifyUser
    {
        public static void UpdateProfile(UserProfile userProfile)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserProfile table
                    string updateUserProfileSql = "UPDATE UserProfile SET FirstName = @FirstName, LastName = @LastName, DOB = @DOB WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(updateUserProfileSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", userProfile.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", userProfile.LastName);
                        cmd.Parameters.AddWithValue("@DOB", userProfile.DOB);
                        cmd.Parameters.AddWithValue("@Username", userProfile.Username); // Add the username parameter

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new Exception("No rows were updated. UserProfile not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // You can add more specific exception handling if needed
                    throw new Exception($"Error updating UserProfile: {ex.Message}");
                }
            }
        }

        public static UserProfile GetUserProfile(string username)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string selectUserProfileSql = "SELECT * FROM UserProfile WHERE Username = @Username";
                using (MySqlCommand cmd = new MySqlCommand(selectUserProfileSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserProfile(
                                reader["Username"].ToString(),
                                reader["FirstName"].ToString(),
                                reader["LastName"].ToString(),
                                Convert.ToDateTime(reader["DOB"])
                            );
                        }
                    }
                }
            }
            return null;
        }
    }
}
