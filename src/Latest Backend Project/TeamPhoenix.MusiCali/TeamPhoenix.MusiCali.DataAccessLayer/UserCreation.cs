using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using _loggerCreation = TeamPhoenix.MusiCali.Logging.Logger;
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

        public static bool CreateUser(UserAccount userAccount, UserAuthN userAuthN, UserRecovery userRecovery, UserClaims userClaims, UserProfile userProfile)
        {
            try
            {
                //string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
                var dao = new SqlDAO("julie", "j1234");

                // Insert data into UserAccount table
                string insertUserAccountSql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES (@Username, @Salt, @UserHash, @Email)";
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                // Adding each parameter into a dictionary
                parameters.Add("@Username", userAccount.Username);
                parameters.Add("@Salt", userAccount.Salt);
                parameters.Add("@UserHash", userAccount.UserHash);
                parameters.Add("@Email", userAccount.Email);
                dao.ExecuteSql(insertUserAccountSql, parameters);

                // Insert data into UserAuthN table
                string insertUserAuthNSql = "INSERT INTO UserAuthN (Username, Salt, OTP, otpTimestamp, FailedAttempts, FirstFailedAttemptTime, IsDisabled, IsAuth, EmailSent) " +
                                            "VALUES (@Username, @Salt, @OTP, @otpTimestamp, @FailedAttempts, @FirstFailedAttemptTime, @IsDisabled, @IsAuth, @EmailSent)";

                Dictionary<string, object> authNParameters = new Dictionary<string, object>();
                // Adding each parameter into a dictionary
                authNParameters.Add("@Username", userAuthN.Username);
                authNParameters.Add("@Salt", userAuthN.Salt);
                authNParameters.Add("@OTP", userAuthN.OTP);
                authNParameters.Add("@otpTimestamp", userAuthN.otpTimestamp);
                authNParameters.Add("@FailedAttempts", userAuthN.FailedAttempts);
                authNParameters.Add("@FirstFailedAttemptTime", userAuthN.FirstFailedAttemptTime);
                authNParameters.Add("@IsDisabled", userAuthN.IsDisabled);
                authNParameters.Add("@IsAuth", userAuthN.IsAuth);
                authNParameters.Add("@EmailSent", userAuthN.EmailSent);
                dao.ExecuteSql(insertUserAuthNSql, authNParameters);

                // Insert data into UserRecovery table
                string insertUserRecoverySql = "INSERT INTO UserRecovery (Username, backupEmail, SuccessRecovery) VALUES (@Username, @backupEmail, @SuccessRecovery)";

                Dictionary<string, object> userRecoveryParameters = new Dictionary<string, object>();
                // Adding each parameter into a dictionary
                userRecoveryParameters.Add("@Username", userRecovery.Username);
                userRecoveryParameters.Add("@backupEmail", userRecovery.backupEmail);
                userRecoveryParameters.Add("@SuccessRecovery", userRecovery.Success);
                dao.ExecuteSql(insertUserRecoverySql, userRecoveryParameters);

                // Insert data into UserClaims table
                string insertUserClaimsSql = "INSERT INTO UserClaims (Username, Claims) VALUES (@Username, @Claims)";
                string claimsJson = JsonConvert.SerializeObject(userClaims.Claims);

                Dictionary<string, object> userClaimsParameters = new Dictionary<string, object>();
                userClaimsParameters.Add("@Username", userClaims.Username);
                userClaimsParameters.Add("@Claims", claimsJson);
                dao.ExecuteSql(insertUserClaimsSql, userClaimsParameters);

                // Insert data into UserProfile table
                string insertUserProfileSql = "INSERT INTO UserProfile (Username, FirstName, LastName, DOB) VALUES (@Username, @FirstName, @LastName, @DOB)";
                Dictionary<string, object> userProfileParameters = new Dictionary<string, object>();
                userProfileParameters.Add("@Username", userProfile.Username);
                userProfileParameters.Add("@FirstName", userProfile.FirstName);
                userProfileParameters.Add("@LastName", userProfile.LastName);
                userProfileParameters.Add("@DOB", userProfile.DOB);
                dao.ExecuteSql(insertUserProfileSql, userProfileParameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.ToString()}");
                return false;
            }


            //_loggerCreation loggerCreation = new _loggerCreation();
            string level = "Info";
            string category = "View";
            string context = "Creating new user";
            _loggerCreation.CreateLog(userAccount.UserHash, level, category, context);

            Console.WriteLine("Data inserted successfully!");
            return true;
        }
    }
}