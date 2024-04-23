using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{

    [TestClass]
    public class AuthenticationTest
    {

        UserCreationService userCreationService = UserCreationService();

        private static UserCreationService UserCreationService()
        {
            throw new NotImplementedException();
        }

        AuthenticationDAO authenticationDao = new AuthenticationDAO();
        UserDeletionService userDeletionService = new UserDeletionService();
        private const string ConnectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

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
            AuthResult authResult = authenticationDao.findUsernameInfo(username);

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
            bool result = authenticationDao.updateAuthentication(userAuthN);

            // Assert
            Assert.IsTrue(result);

            // Add more specific assertions if needed
        }

        [TestCleanup]
        public void Cleanup()
        {
            /*
            Tester.DeleteAllRows("UserAuthN");
            Tester.DeleteAllRows("UserProfile");
            Tester.DeleteAllRows("UserRecovery");
            Tester.DeleteAllRows("UserClaims");
            Tester.DeleteAllRows("UserAccount");
            */
        }
        // Add more test methods for other scenarios

        // Example: Additional test methods for logAuthFailure, etc.
    }
}
