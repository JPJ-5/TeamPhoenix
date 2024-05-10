using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;
using Microsoft.Extensions.Configuration;

namespace Teamphoenix.Musicali.Tests
{
    [TestClass]
    public class AuthenticationTest
    {
        private readonly IConfiguration configuration;
        private UserCreationService userCreationService;
        private UserDeletionService userDeletionService;
        private AuthenticationDAO authenticationDAO;

        public AuthenticationTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();
            userCreationService = new UserCreationService(configuration);
            userDeletionService = new UserDeletionService(configuration);
            authenticationDAO = new AuthenticationDAO(configuration);
        }
        [TestMethod]
        public void FindUsernameInfo_ShouldReturnValidAuthResult()
        {
            // Arrange
            string email = "test@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";
            string backupEmail = "";

            // Act
            userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);//change this
            // Arrange

            // Act
            AuthResult authResult = authenticationDAO.findUsernameInfo(username);

            // Assert
            Assert.IsNotNull(authResult);
            Assert.IsNotNull(authResult.userA);
            Assert.IsNotNull(authResult.userAcc);
            Assert.IsNotNull(authResult.userC);

            // Add more specific assertions based on your data structure
            // For example, you may check specific properties of userA, userAcc, and userC
        }

        [TestMethod]
        public void UpdateAuthentication_ShouldUpdateUserAuthN()
        {
            string email = "test@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";
            string backupEmail = "test2@example.com";

            // Act
            userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            // Arrange
            UserAuthN userAuthN = new UserAuthN("testuser", "newotp", DateTime.UtcNow, "newsalt");
            // Act
            bool result = authenticationDAO.updateAuthentication(userAuthN);

            // Assert
            Assert.IsTrue(result);

            // Add more specific assertions if needed
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up the user created during the test
            userDeletionService.DeleteUser("testuser");
        }

    }
}
