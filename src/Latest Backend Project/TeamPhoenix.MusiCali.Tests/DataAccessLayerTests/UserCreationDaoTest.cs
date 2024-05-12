using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class UserCreationDaoTest
    {
        private readonly IConfiguration configuration;
        private UserCreationDAO userCreationDAO;
        private UserDeletionService userDeletionService;
        public UserCreationDaoTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            userCreationDAO = new UserCreationDAO(configuration);
            userDeletionService = new UserDeletionService(configuration);
        }



        [TestMethod]
        public void CreateUser()
        {
            Dictionary<string, string> claims = new Dictionary<string, string>
            {
                {"UserRole", "User"}
            };
            string email = "test@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";
            UserAccount userAccount = new UserAccount(username, "testsalt", "fakehash", email);
            UserAuthN userAuth = new UserAuthN(username, "testotp", DateTime.Now, "testsalt");
            UserRecovery userR = new UserRecovery(username, email);
            UserClaims userC = new UserClaims(username, claims);

            UserProfile userP = new UserProfile(username, "prof", "vong", dateOfBirth);

            // Act
            userCreationDAO.CreateUser(userAccount, userAuth, userR, userC, userP);


            // Assert - Check if the user is now registered
            Assert.IsTrue(userCreationDAO.IsUsernameRegistered(userAccount.Username));
            userDeletionService.DeleteUser(userAccount.Username);
        }

        [TestMethod]
        public void Isusername_email_saltRegistered()
        {
            Dictionary<string, string> claims = new Dictionary<string, string>
            {
                {"UserRole", "User"}
            };
            string email = "test@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";
            UserAccount userAccount = new UserAccount(username, "testsalt", "fakehash", email);
            UserAuthN userAuth = new UserAuthN(username, "testotp", DateTime.Now, "testsalt");
            UserRecovery userR = new UserRecovery(username, email);
            UserClaims userC = new UserClaims(username, claims);

            UserProfile userP = new UserProfile(username, "prof", "vong", dateOfBirth);

            // Act
            userCreationDAO.CreateUser(userAccount, userAuth, userR, userC, userP);


            // Assert - Check if the user is now registered
            Assert.IsTrue(userCreationDAO.IsEmailRegistered(userAccount.Email));
            Assert.IsTrue(userCreationDAO.IsUsernameRegistered(userAccount.Username));
            Assert.IsTrue(userCreationDAO.IsSaltUsed(userAccount.Salt));
            userDeletionService.DeleteUser(userAccount.Username);
        }


        [TestMethod]
        public void IsEmailRegistered_ShouldReturnFalseForNonRegisteredEmail()
        {
            // Arrange
            var email = "nonexistent@email.com"; // Replace with a non-existing email in your database

            // Act
            var result = userCreationDAO.IsEmailRegistered(email);

            // Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void IsUsernameRegistered_ShouldReturnFalseForNonRegisteredUsername()
        {
            // Arrange
            var username = "nonexistentuser"; // Replace with a non-existing username in your database

            // Act
            var result = userCreationDAO.IsUsernameRegistered(username);

            // Assert
            Assert.IsFalse(result);
        }



        [TestMethod]
        public void IsSaltUsed_ShouldReturnFalseForUnusedSalt()
        {
            // Arrange
            var salt = "unusedsalt"; // Replace with an unused salt in your database

            // Act
            var result = userCreationDAO.IsSaltUsed(salt);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
