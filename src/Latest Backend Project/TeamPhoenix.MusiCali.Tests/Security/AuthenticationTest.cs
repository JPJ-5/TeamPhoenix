using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using hash = TeamPhoenix.MusiCali.Security.Hasher;
using TeamPhoenix.MusiCali.DataAccessLayer;

/*
namespace TeamPhoenix.MusiCali.Tests.Security
{
    [TestClass]
    public class AuthenticationTest
    {
        private const string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

        [TestMethod]
        public void Authenticate_WithValidCredentials_ShouldReturnPrincipal()
        {
            // Arrange
            var authentication = new TeamPhoenix.MusiCali.Security.Authentication();
            UserAuthN userAuthN = new UserAuthN("testuser", hash.HashPassword("testotp", "testsalt"), DateTime.Now, "testsalt");
            UserAccount userAcc = new UserAccount("testuser", "testsalt", hash.HashPassword("testuser", "testsalt"), "testemail");
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Insert data into UserAccount table
                    string insertUserAccountSql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES (@Username, @Salt, @UserHash, @Email)";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserAccountSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userAcc.Username);
                        cmd.Parameters.AddWithValue("@Salt", userAcc.Salt);
                        cmd.Parameters.AddWithValue("@UserHash", userAcc.UserHash);
                        cmd.Parameters.AddWithValue("@Email", userAcc.Email);
                        cmd.ExecuteNonQuery();
                    }

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
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error inserting UserAuthN {ex.Message}");
                }
            }

            // Mock or set up your database with valid user credentials for testing
            string validUsername = "testuser";
            string validPassword = "testotp";

            // Act
            Principal principal = authentication.Authenticate(validUsername, validPassword);

            // Assert
            Assert.IsNotNull(principal);
            // Add more assertions as needed based on your specific implementation
        }

        [TestMethod]
        public void Authenticate_WithInvalidUsername_ShouldThrowArgumentException()
        {
            // Arrange
            var authentication = new TeamPhoenix.MusiCali.Security.Authentication();

            // Mock or set up your database with invalid username for testing
            string invalidUsername = "invalidUser";
            string validPassword = "validPassword";

            // Act and Assert
            Assert.ThrowsException<Exception>(() => authentication.Authenticate(invalidUsername, validPassword));
        }

        [TestCleanup]
        public void Cleanup()
        {
            Tester.DeleteAllRows("UserAuthN");
            Tester.DeleteAllRows("UserProfile");
            Tester.DeleteAllRows("UserRecovery");
            Tester.DeleteAllRows("UserClaims");
            Tester.DeleteAllRows("UserAccount");
        }

    }
}
*/