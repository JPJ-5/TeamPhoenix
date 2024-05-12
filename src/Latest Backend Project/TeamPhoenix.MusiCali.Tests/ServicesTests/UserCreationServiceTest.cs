using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class UserCreationServiceTest
    {
        private readonly IConfiguration configuration;
        private UserCreationService userCreationService;
        private UserDeletionService userDeletionService;
        public UserCreationServiceTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            userCreationService = new UserCreationService(configuration);
            userDeletionService = new UserDeletionService(configuration);
        }

        [TestMethod]
        public void RegisterUser_ShouldReturnTrueForValidRegistration()
        {
            // Arrange
            string email = "test1234@example.com";
            string backupEmail = "backuestemailtry@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser123";

            // Act
            bool result = userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);

            // Assert
            Assert.IsTrue(result);
            userDeletionService.DeleteUser(username);
        }

        [TestMethod]
        public void RegisterUser_ShouldThrowArgumentExceptionForInvalidEmail()
        {
            // Arrange
            string email = "test2@example.com";
            string backupEmail = "backupTest2@example.com";
            DateTime dateOfBirth = new DateTime(1950, 1, 1);
            string username = "testuser2";
            // Act
            bool result = userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RegisterUser_ShouldThrowArgumentExceptionForInvalidDateOfBirth()
        {
            string email = "test3@example.com";
            string backupEmail = "backupTest3@example.com";
            DateTime dateOfBirth = new DateTime(1669, 1, 1);
            string username = "testuser3";

            // Act
            bool result = userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            // Assert
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void RegisterUser_ShouldThrowArgumentExceptionForInvalidUsername()
        {
            string email = "test4@example.com";
            string backupEmail = "backupTest4@example.com";
            DateTime dateOfBirth = new DateTime(1800, 1, 1);
            string username = "testuser4!"; //should return an invalid username

            // Act
            bool result = userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            // Assert
            Assert.IsFalse(result);
        }

    }
}