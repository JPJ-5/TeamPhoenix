using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;


namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class RecoverUserDAO
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;
        private AuthenticationDAO authenticationDAO;
        public RecoverUserDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
            authenticationDAO = new AuthenticationDAO(this.configuration);
        }
        public UserRecovery GetUserRecovery(string username)
        {

            try
            {   
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string selectUserAccountSql = "SELECT * FROM UserRecovery WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(selectUserAccountSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("0");
                            if (reader.Read())
                            {
                                return new UserRecovery(
                                    reader["Username"].ToString()!,
                                    reader["backupEmail"].ToString()!);
                            }
                        }
                        
                    }
                    return new UserRecovery();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new UserRecovery();
            }
        }

        public string GetOTP(string username)
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

        public string GetUserHash(string username)
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

        public bool updateAccountRecovery(UserAccount userAcc)
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

        public bool updateAuth(UserAuthN userAuthN)
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

        public UserAccount findUserAccount(string username)
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
        public bool updateUserR(UserRecovery userR)
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
                UserAuthN theUser = authenticationDAO.findUsernameInfo(userR.Username).userA!;

                if (!EnableUser(theUser))
                {
                    return false;
                }
                return true;
            }
        }

        public bool DisableUser(UserAuthN userAuthN)
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
                return true;
            }
        }

        public bool EnableUser(UserAuthN userAuthN)
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
                return true;
            }

        }

        public Boolean checkUserName(string username)
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
    }
}
