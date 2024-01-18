using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class UserCreation
    {
        public static bool IsEmailRegistered(string email)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Check if the username or email already exists in UserAccount table
                string checkDuplicateSql = "SELECT COUNT(*) FROM UserAccount WHERE Email = @Email";
                using (MySqlCommand cmd = new MySqlCommand(checkDuplicateSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }

            }
        }

        public static bool IsUsernameRegistered(string username)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Check if the username or email already exists in UserAccount table
                string checkDuplicateSql = "SELECT COUNT(*) FROM UserAccount WHERE Username = @Username";
                using (MySqlCommand cmd = new MySqlCommand(checkDuplicateSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }

            }
        }

        public static bool IsSaltUsed(string salt)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Check if the username or email already exists in UserAccount table
                string checkDuplicateSql = "SELECT COUNT(*) FROM UserAccount WHERE Salt = @Salt";
                using (MySqlCommand cmd = new MySqlCommand(checkDuplicateSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Salt", salt);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }

            }
        }

        public static void CreateUser(UserAccount userAccount, UserAuthN userAuthN, UserRecovery userRecovery, UserClaims userClaims, UserProfile userProfile)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Insert data into UserAccount table
                    string insertUserAccountSql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES (@Username, @Salt, @UserHash, @Email)";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserAccountSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userAccount.Username);
                        cmd.Parameters.AddWithValue("@Salt", userAccount.Salt);
                        cmd.Parameters.AddWithValue("@UserHash", userAccount.UserHash);
                        cmd.Parameters.AddWithValue("@Email", userAccount.Email);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert data into UserAuthN table
                    string insertUserAuthNSql = "INSERT INTO UserAuthN (Username, Salt, OTP, Password, otpTimestamp, FailedAttempts, FirstFailedAttemptTime, IsDisabled, IsAuth, EmailSent) " +
                                                "VALUES (@Username, @Salt, @OTP, @Password, @otpTimestamp, @FailedAttempts, @FirstFailedAttemptTime, @IsDisabled, @IsAuth, @EmailSent)";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserAuthNSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userAuthN.Username);
                        cmd.Parameters.AddWithValue("@Salt", userAuthN.Salt);
                        cmd.Parameters.AddWithValue("@OTP", userAuthN.OTP);
                        cmd.Parameters.AddWithValue("@Password", userAuthN.Password);
                        cmd.Parameters.AddWithValue("@otpTimestamp", userAuthN.otpTimestamp);
                        cmd.Parameters.AddWithValue("@FailedAttempts", userAuthN.FailedAttempts);
                        cmd.Parameters.AddWithValue("@FirstFailedAttemptTime", userAuthN.FirstFailedAttemptTime);
                        cmd.Parameters.AddWithValue("@IsDisabled", userAuthN.IsDisabled);
                        cmd.Parameters.AddWithValue("@IsAuth", userAuthN.IsAuth);
                        cmd.Parameters.AddWithValue("@EmailSent", userAuthN.EmailSent);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert data into UserRecovery table
                    string insertUserRecoverySql = "INSERT INTO UserRecovery (Username, Question, Answer, SuccessRecovery) VALUES (@Username, @Question, @Answer, @SuccessRecovery)";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserRecoverySql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userRecovery.Username);
                        cmd.Parameters.AddWithValue("@Question", userRecovery.Question);
                        cmd.Parameters.AddWithValue("@Answer", userRecovery.Answer);
                        cmd.Parameters.AddWithValue("@SuccessRecovery", userRecovery.Success);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert data into UserClaims table
                    string insertUserClaimsSql = "INSERT INTO UserClaims (Username, Claims) VALUES (@Username, @Claims)";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserClaimsSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userClaims.Username);
                        cmd.Parameters.AddWithValue("@Claims", userClaims.Claims);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert data into UserProfile table
                    string insertUserProfileSql = "INSERT INTO UserProfile (Username, FirstName, LastName, DOB) VALUES (@Username, @FirstName, @LastName, @DOB)";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserProfileSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userProfile.Username);
                        cmd.Parameters.AddWithValue("@FirstName", userProfile.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", userProfile.LastName);
                        cmd.Parameters.AddWithValue("@DOB", userProfile.DOB);
                        cmd.ExecuteNonQuery();
                    }

                    Console.WriteLine("Data inserted successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}. Retry or contact admin");
                }
            }
        }
    }
}