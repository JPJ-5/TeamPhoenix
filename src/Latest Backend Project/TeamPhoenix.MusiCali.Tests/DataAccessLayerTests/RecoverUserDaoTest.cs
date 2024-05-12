using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;

namespace Teamphoenix.Musicali.Tests
{
    [TestClass]
    public class RecoverUserDaoTest
    {
        private readonly IConfiguration configuration;
        private RecoverUserDAO recoverUserDAO;
        private UserCreationDAO userCreationDAO;
        private UserDeletionService userDeletionService;

        public RecoverUserDaoTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            recoverUserDAO = new RecoverUserDAO(configuration);
            userCreationDAO = new UserCreationDAO(configuration);
            userDeletionService = new UserDeletionService(configuration);
        }
        [TestMethod]
        public void GetUserRecovery_ShouldReturnUserRecoveryObject()
        {
            // Arrange
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

            // Act
            UserRecovery result = recoverUserDAO.GetUserRecovery(username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(username, result.Username);
            userDeletionService.DeleteUser(username);
        }

        [TestMethod]
        public void UpdateUserRecovery_ShouldReturnTrueForSuccessfulUpdate()
        {
            Dictionary<string, string> claims = new Dictionary<string, string>
            {
                {"UserRole", "User"}
            };
            string email = "test@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";
            UserAccount userAccount = new UserAccount(username, "testsalt", "fakehash", "email");
            UserAuthN userAuth = new UserAuthN(username, "testotp", DateTime.Now, "testsalt");
            UserRecovery userR = new UserRecovery(username, email);
            UserClaims userC = new UserClaims(username, claims);

            UserProfile userP = new UserProfile(username, "prof", "vong", dateOfBirth);

            // Act
            userCreationDAO.CreateUser(userAccount, userAuth, userR, userC, userP);
            UserRecovery userRecovery = new UserRecovery(username, email);
            userRecovery.Success = true;

            // Act
            bool result = recoverUserDAO.updateUserR(userRecovery);

            // Assert
            Assert.IsTrue(result);
            userDeletionService.DeleteUser(username);
        }

    }
}
