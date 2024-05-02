using MySql.Data.MySqlClient;
using Newtonsoft.Json; // namespace for JSON serialization
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class ModifyUserDAO
    {
        private readonly string? connectionString;
        private readonly IConfiguration? configuration;

        public ModifyUserDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }

        public ModifyUserDAO()
        {
        }

        public void UpdateProfile(UserProfile userProfile)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserProfile table
                    string updateUserProfileSql = "UPDATE UserProfile SET FirstName = @FirstName, LastName = @LastName WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(updateUserProfileSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", userProfile.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", userProfile.LastName);
                        //cmd.Parameters.AddWithValue("@DOB", userProfile.DOB);
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

        public UserProfile GetProfile(string username)
        {
            string query = "SELECT Username, FirstName, LastName, DOB FROM UserProfile WHERE Username = @Username";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserProfile(
                                reader["Username"].ToString() ?? string.Empty,
                                reader["FirstName"].ToString() ?? string.Empty,
                                reader["LastName"].ToString() ?? string.Empty,
                                Convert.ToDateTime(reader["DOB"])
                                );
                        }
                        else
                        {
                            throw new KeyNotFoundException($"A user profile with the username '{username}' could not be found.");
                        }
                    }
                }
            }
        }

        public bool ModifyProfile(string username, string firstName, string lastName)
        {
            string query = "UPDATE UserProfile SET FirstName = @FirstName, LastName = @LastName WHERE Username = @Username";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Username", username);
                        var result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateClaims(string username, Dictionary<string, string> updatedClaims)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE UserClaims SET Claims = @Claims WHERE Username = @Username";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Serialize the updated Claims dictionary to a JSON string
                        string claimsJson = JsonConvert.SerializeObject(updatedClaims);

                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Claims", claimsJson);

                        var result = command.ExecuteNonQuery();
                        return result > 0; // True if at least one row was updated
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions or log them
                return false;
            }
        }

        public object GetUserInformation(string username)
        {
            string query = @"
                SELECT 
                    up.FirstName, 
                    up.LastName, 
                    up.DOB, 
                    ua.Email,
                    uc.Claims,
                    IF(ua.Username IS NOT NULL, 'Active', 'Inactive') AS UserStatus
                FROM UserProfile up
                LEFT JOIN UserAccount ua ON up.Username = ua.Username
                LEFT JOIN UserClaims uc ON up.Username = uc.Username
                WHERE up.Username = @Username";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var claimsJson = reader["Claims"].ToString();
                            var claimsDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(claimsJson!)!;
                            string userRole = "Unknown";
                            userRole = claimsDict["UserRole"];

                            // Handle nullable FirstName and LastName
                            var firstName = reader["FirstName"] as string; // Using 'as' for safe casting that returns null for DBNull
                            var lastName = reader["LastName"] as string; // Same here

                            // Directly return an anonymous object without needing a dedicated class
                            return new
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                DateOfBirth = Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd"),
                                Email = reader["Email"].ToString(),
                                UserStatus = reader["UserStatus"].ToString(),
                                UserRole = userRole
                            };
                        }
                        else
                        {
                            throw new KeyNotFoundException($"User with username '{username}' could not be found.");
                        }
                    }
                }
            }
        }
    }
}