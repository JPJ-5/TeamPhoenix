using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class RecoveryTest
    {

        private readonly IConfiguration configuration;
        private RecoverUserService recoverUserService;
        private UserCreationDAO userCreationDAO;
        private UserDeletionService userDeletionService;
        public RecoveryTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            recoverUserService = new RecoverUserService(configuration);
            userCreationDAO = new UserCreationDAO(configuration);
            userDeletionService = new UserDeletionService(configuration);
        }


        [TestMethod]
        public void RecoverDisabledAccount_ShouldReturnTrueForSuccessfulRecovery()
        {
            Dictionary<string, string> claims = new Dictionary<string, string>
            {
                {"UserRole", "User"}
            };
            DateTime dob = new DateTime(2001, 01, 26);
            UserAccount userAccount = new UserAccount("username", "testsalt", "fakehash", "email");
            UserAuthN userAuth = new UserAuthN("username", "testotp", DateTime.Now, "testsalt");
            UserRecovery userR = new UserRecovery("username", "backupEmail");
            UserClaims userC = new UserClaims("username", claims);

            UserProfile userP = new UserProfile("username", "prof", "vong", dob);

            // Act
            userCreationDAO.CreateUser(userAccount, userAuth, userR, userC, userP);

            // Arrange
            string username = "username";

            // Act
            bool result = recoverUserService.EnableUser(username);

            // Assert
            Assert.IsTrue(result);
            userDeletionService.DeleteUser(username);
        }

        [TestMethod]
        public void IsValidUsername_ShouldReturnTrueForValidUsername()
        {
            // Arrange
            string validUsername = "validusername";

            // Act
            bool result = recoverUserService.isValidUsername(validUsername);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateOTP_ShouldReturnTrueForMatching()
        {
            // Arrange
            string OTP = "TestAnswer";
            string storedOTP = "TestAnswer";

            // Act
            bool result = recoverUserService.ValidateOTP(OTP, storedOTP);

            // Assert
            Assert.IsTrue(result);
        }

    }
}