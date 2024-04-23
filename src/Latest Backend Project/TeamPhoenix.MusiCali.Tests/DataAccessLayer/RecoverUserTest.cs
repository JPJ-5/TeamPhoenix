using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{
    [TestClass]
    public class RecoverUserTest
    {
        UserCreationDAO UserCreationDao = new UserCreationDAO();
        RecoverUserDAO recoverUserDAO = new RecoverUserDAO();
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
            UserCreationDao.CreateUser(userAccount, userAuth, userR, userC, userP);

            // Act
            UserRecovery result = recoverUserDAO.GetUserRecovery(username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(username, result.Username);
            // Add more assertions based on the expected data in the UserRecovery object
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
            UserCreationDao.CreateUser(userAccount, userAuth, userR, userC, userP);
            UserRecovery userRecovery = new UserRecovery(username, email);
            userRecovery.Success = true;

            // Act
            bool result = recoverUserDAO.updateUserR(userRecovery);

            // Assert
            Assert.IsTrue(result);
        }

        // Add more test methods for other scenarios

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
    }
}
