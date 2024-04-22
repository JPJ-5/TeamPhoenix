using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class Authentication
    {
        public static Result? logAuthFailure(string error)
        {
            return null;
        }

        public static AuthResult findUsernameInfo(string username)
        {
            AuthResult authResult = new AuthResult(); // Create an instance of AuthResult

            try
            {
                string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
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

        public static bool updateAuthentication(UserAuthN userAuthN)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

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

        private static Dictionary<string, string> ParseClaimsFromJson(string claimsJson)
        {
            // Implement logic to deserialize JSON or parse the string to create a Dictionary<string, string>
            // You may use a JSON library like Newtonsoft.Json for deserialization
            // For simplicity, I'm providing a basic example assuming a simple key-value pair format
            // Adjust this based on your actual data structure

            Dictionary<string, string> claims = new Dictionary<string, string>();

            // Parse the claimsJson string and populate the claims dictionary

            return claims;
        }
    }
}

