using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using dao = TeamPhoenix.MusiCali.DataAccessLayer.Authentication;
using uc = TeamPhoenix.MusiCali.Services.UserCreation;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{
    [TestClass]
    public class AuthenticationTest
    {
        private const string ConnectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

        [TestMethod]
        public void FindUsernameInfo_ShouldReturnValidAuthResult()
        {
            // Arrange
            string email = "test123@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser123";
            string fname = "John";
            string lname = "Doe";
            string q = "Security Question";
            string a = "a";

            // Act
            uc.RegisterUser(email, dateOfBirth, username, fname, lname, q, a);
            // Arrange

            // Act 
            AuthResult authResult = dao.findUsernameInfo(username);

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
            string fname = "John";
            string lname = "Doe";
            string q = "Security Question";
            string a = "a";

            // Act
            uc.RegisterUser(email, dateOfBirth, username, fname, lname, q, a);
            // Arrange
            UserAuthN userAuthN = new UserAuthN("testuser", "newotp", DateTime.UtcNow, "newsalt");
            // Act
            bool result = dao.updateAuthentication(userAuthN);

            // Assert
            Assert.IsTrue(result);

            // Add more specific assertions if needed
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
        // Add more test methods for other scenarios

        // Example: Additional test methods for logAuthFailure, etc.
    }
}
