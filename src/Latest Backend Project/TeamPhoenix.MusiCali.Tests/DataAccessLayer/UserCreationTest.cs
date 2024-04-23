using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using static System.Net.WebRequestMethods;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{
    [TestClass]
    public class UserCreationTest
    {
        UserCreationDAO userCreationDAO = new UserCreationDAO();
        private const string ConnectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

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

        // Add similar test methods for other scenarios

        // Example: Additional CreateUser method tests could be added as well
    }
}
