using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class AuthenticationDAO
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;

        public AuthenticationDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }
        public Result? logAuthFailure(string error)
        {
            return null;
        }

        public AuthResult findUsernameInfo(string username)
        {
            AuthResult authResult = new AuthResult(); // Create an instance of AuthResult

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string selectUserAuthNSql = "SELECT * FROM UserAuthN WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(selectUserAuthNSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserAuthN userA = new UserAuthN(
                                    reader["Username"].ToString()!,
                                    reader["Salt"].ToString()!,
                                    reader["OTP"].ToString()!,
                                    Convert.ToDateTime(reader["otpTimestamp"]),
                                    Convert.ToInt32(reader["FailedAttempts"]),
                                    Convert.ToDateTime(reader["FirstFailedAttemptTime"]),
                                    Convert.ToBoolean(reader["IsDisabled"]),
                                    Convert.ToBoolean(reader["IsAuth"]),
                                    Convert.ToBoolean(reader["EmailSent"])
                                );
                                authResult.userA = userA;
                            }
                        }
                    }

                    string selectUserAccountSql = "SELECT * FROM UserAccount WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(selectUserAccountSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserAccount UserAcc = new UserAccount(
                                    reader["Username"].ToString()!,
                                    reader["Salt"].ToString()!,
                                    reader["UserHash"].ToString()!,
                                    reader["Email"].ToString()!
                                );
                                authResult.userAcc = UserAcc;
                            }
                        }
                    }

                    string selectUserClaimsSql = "SELECT * FROM UserClaims WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(selectUserClaimsSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Assuming the 'Claims' column in the database is a string representation of JSON or similar
                                string claimsJson = reader["Claims"].ToString()!;

                                // Deserialize the JSON or parse the string to create a Dictionary<string, string>
                                Dictionary<string, string> claims = ParseClaimsFromJson(claimsJson);

                                UserClaims userC = new UserClaims(
                                    reader["Username"].ToString()!,
                                    claims
                                );
                                authResult.userC = userC;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user {ex.Message}");
            }
            return authResult;
        }

        public bool updateAuthentication(UserAuthN userAuthN)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserAuthN table
                    string updateUserAuthNSql = "UPDATE UserAuthN SET OTP = @OTP, otpTimestamp = @otpTimestamp, Salt = @Salt, IsAuth = @IsAuth WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(updateUserAuthNSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userAuthN.Username);
                        cmd.Parameters.AddWithValue("@OTP", userAuthN.OTP);
                        cmd.Parameters.AddWithValue("@otpTimestamp", userAuthN.otpTimestamp);
                        cmd.Parameters.AddWithValue("@Salt", userAuthN.Salt);
                        cmd.Parameters.AddWithValue("@IsAuth", userAuthN.IsAuth);
                        cmd.ExecuteNonQuery();
                    }
                    

                    return true;
                }
                catch (Exception ex)
                {
                    // You can add more specific exception handling if needed
                    throw new Exception($"Error updating UserAuthN: {ex.Message}");
                }
            }
        }

        public bool DeauthenticateUser(string username)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "UPDATE UserAuthN SET IsAuth = 0 WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        int result = cmd.ExecuteNonQuery();
                        return result > 0; // Returns true if any rows were updated
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the error as needed
                    throw new Exception($"Error deauthenticating user: {ex.Message}");
                }
            }
        }

        private Dictionary<string, string> ParseClaimsFromJson(string claimsJson)
        {
            try
            {
                // Deserialize the JSON string into a Dictionary
                var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(claimsJson);
                return claims ?? new Dictionary<string, string>(); // Return an empty dictionary if null
            }
            catch (JsonException ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error parsing claims JSON: " + ex.Message);
            }
        }
    }
}

