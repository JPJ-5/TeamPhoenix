using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class UserDeletionTest
    {

        private readonly IConfiguration configuration;
        private UserCreationService userCreationService;
        private UserDeletionService userDeletionService;
        public UserDeletionTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            userCreationService = new UserCreationService(configuration);
            userDeletionService = new UserDeletionService(configuration);
        }


        [TestMethod]
        public void DeleteUser_ShouldReturnTrueAndDeleteUser()
        {
            // Arrange
            string email = "test1234@example.com";
            string backupEmail = "backuestemailtry@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser123";

            // Act
            bool result = userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            Assert.IsTrue(result);

            bool deleteRes = userDeletionService.DeleteUser(username);

            Assert.IsTrue(deleteRes);

        }

        [TestMethod]
        public void DeleteUser_ShouldReturnFalseForInvalidUsername()
        {
            // Arrange
            string username = "haley";

            // Act

            bool deleteRes = userDeletionService.DeleteUser(username);

            Assert.IsFalse(deleteRes);

        }
    }
}
