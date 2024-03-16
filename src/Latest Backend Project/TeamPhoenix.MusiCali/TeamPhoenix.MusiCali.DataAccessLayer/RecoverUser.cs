using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using _logger = TeamPhoenix.MusiCali.Logging.Logger;
using System.Collections;


namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class RecoverUser
    {
        private static readonly string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

        public static UserRecovery GetUserRecovery(string username)
        {
#pragma warning disable CS8603, CS8604
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string selectUserProfileSql = "SELECT * FROM UserRecovery WHERE Username = @Username";
                using (MySqlCommand cmd = new MySqlCommand(selectUserProfileSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserRecovery(
                                reader["Username"].ToString(),
                                reader["backupEmail"].ToString());
                        }
                    }
                }
                return null;
            }
#pragma warning restore CS8603, CS8604
        }

        public static string GetOTP(string username)
        {
#pragma warning disable CS8603, CS8604
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string selectUserProfileSql = "SELECT OTP FROM UserAuthN WHERE Username = @Username";
                using (MySqlCommand cmd = new MySqlCommand(selectUserProfileSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string userOTP = new string(
                                reader["OTP"].ToString()
                            );
                            return userOTP;
                        }
                    }
                }
            }
            return null;
#pragma warning restore CS8603, CS8604
        }

        public static string GetUserHash(string username)
        {
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
                            string userH = new string(
                                reader["UserHash"].ToString()
                            );
                            return userH;
                        }
                    }
                }
            }
            return "";
        }

        public static bool updateAccountRecovery(UserAccount userAcc)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserAuthN table
                    string updateUserAccSql = "UPDATE UserAccount SET Email = @Email WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(updateUserAccSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userAcc.Username);
                        cmd.Parameters.AddWithValue("@Email", userAcc.Email);
                        cmd.ExecuteNonQuery();
                    }


                    return true;
                }
                catch (Exception ex)
                {
                    // You can add more specific exception handling if needed
                    throw new Exception($"Error updating UserAcc: {ex.Message}");
                }
            }
        }

        public static bool updateAuth(UserAuthN userAuthN)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserAuthN table
                    string updateUserAuthNSql = "UPDATE UserAuthN SET OTP = @OTP, otpTimestamp = @otpTimestamp WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(updateUserAuthNSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userAuthN.Username);
                        cmd.Parameters.AddWithValue("@OTP", userAuthN.OTP);
                        cmd.Parameters.AddWithValue("@otpTimestamp", userAuthN.otpTimestamp);
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

        public static UserAccount findUserAccount(string username)
        {
            UserAccount userAcc = new UserAccount(); // Create an instance of AuthResult

            try
            {
                string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

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
                                userAcc = UserAcc;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user {ex.Message}");
            }
            return userAcc;
        }
        public static bool updateUserR(UserRecovery userR)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserProfile table
                    string insertUserRecoverySql = "UPDATE UserRecovery SET Username = @Username, backupEmail = @backupEmail, SuccessRecovery = @SuccessRecovery WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserRecoverySql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userR.Username);
                        cmd.Parameters.AddWithValue("@backupEmail", userR.backupEmail);
                        cmd.Parameters.AddWithValue("@SuccessRecovery", userR.Success);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    // You can add more specific exception handling if needed
                    throw new Exception($"Error updating UserProfile: {ex.Message}");
                }
                UserAuthN theUser = Authentication.findUsernameInfo(userR.Username).userA!;

                if (!EnableUser(theUser))
                {
                    return false;
                }
                string userHash = GetUserHash(userR.Username);
                string level = "Info";
                string category = "View";
                string context = "Recover User";
                _logger.CreateLog(userHash, level, category, context);
                return true;
            }
        }

        public static bool DisableUser(UserAuthN userAuthN)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserProfile table
                    string insertUserAuthNSQL = "UPDATE UserAuthN SET IsDisabled = @IsDisabled WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserAuthNSQL, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userAuthN.Username);
                        cmd.Parameters.AddWithValue("@IsDisabled", true);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error updating UserAuthN: {ex.Message}");
                }
                string userHash = GetUserHash(userAuthN.Username);
                string level = "Info";
                string category = "View";
                string context = "Disable User";
                _logger.CreateLog(userHash, level, category, context);
                return true;
            }
        }

        public static bool EnableUser(UserAuthN userAuthN)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserProfile table
                    string insertUserAuthNSQL = "UPDATE UserAuthN SET IsDisabled = @IsDisabled WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserAuthNSQL, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userAuthN.Username);
                        cmd.Parameters.AddWithValue("@IsDisabled", false);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error updating UserAuthN: {ex.Message}");
                }
                string userHash = GetUserHash(userAuthN.Username);
                string level = "Info";
                string category = "View";
                string context = "Enable User";
                _logger.CreateLog(userHash, level, category, context);
                return true;
            }

        }

        public static Boolean checkUserName(string username)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserProfile table
                    string readUserAuthNQuery = "SELECT UserHash FROM UserAccount WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(readUserAuthNQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return true;
                            }
                            return false;
                        }

                    }
                }
                catch (Exception ex)
                {
                    // You can add more specific exception handling if needed
                    throw new Exception($"Error getting Username: {ex.Message}");

                }
            }

        }
        //public static List<string> getAllEmail()
        //{
        //    using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    {
        //        List<string> emails = new List<string>();
        //        try
        //        {
        //            connection.Open();

        //            // Update data in UserProfile table
        //            string readUserAuthNQuery = "SELECT Email FROM UserAccount";
        //            using (MySqlCommand cmd = new MySqlCommand(readUserAuthNQuery, connection))
        //            {

        //                using (MySqlDataReader reader = cmd.ExecuteReader())
        //                {
        //                    while (reader.Read()) // Use while instead of if to read all rows
        //                    {
        //                        string email = reader["Email"].ToString();
        //                        emails.Add(email);
        //                    }

                            

        //                }

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // You can add more specific exception handling if needed
        //            Console.WriteLine($"Error getting Username: {ex.Message}");
        //            return null;

        //        }

        //        return emails;
        //    }
        //}
    }
}
