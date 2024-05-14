using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class UserCreationDAO
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;

        public UserCreationDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }

        public bool IsEmailRegistered(string email)
        {
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

        public bool IsUsernameRegistered(string username)
        {
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

        public bool IsSaltUsed(string salt)
        {
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

        public bool CreateUser(UserAccount userAccount, UserAuthN userAuthN, UserRecovery userRecovery, UserClaims userClaims, UserProfile userProfile)
        {
            try
            {
                var dao = new SqlDAO(configuration);

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

                // Insert data into ArtistProfile table
                string insertArtistProfileSql = "INSERT INTO ArtistProfile (Username, ArtistCollabSearchVisibility, ArtistBio, File0Path, File1Path, File2Path, File3Path, File4Path, File5Path) VALUES (@Username, @Visibility, @Bio, @FilePath0, @FilePath1, @FilePath2, @FilePath3, @FilePath4, @FilePath5)";
                Dictionary<string, object> artistProfileParameters = new Dictionary<string, object>();
                artistProfileParameters.Add("@Username", userProfile.Username);
                artistProfileParameters.Add("@Visibility", false); // Set ArtistCollabSearchVisibility to false
                artistProfileParameters.Add("@Bio", "Set your own bio");
                artistProfileParameters.Add("@FilePath0", ""); // Empty string for File0Path
                artistProfileParameters.Add("@FilePath1", ""); // Empty string for File1Path
                artistProfileParameters.Add("@FilePath2", ""); // Empty string for File2Path
                artistProfileParameters.Add("@FilePath3", ""); // Empty string for File3Path
                artistProfileParameters.Add("@FilePath4", ""); // Empty string for File4Path
                artistProfileParameters.Add("@FilePath5", ""); // Empty string for File5Path
                dao.ExecuteSql(insertArtistProfileSql, artistProfileParameters);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.ToString()}");
                return false;
            }
            Console.WriteLine("Data inserted successfully!");
            return true;
        }
    }
}