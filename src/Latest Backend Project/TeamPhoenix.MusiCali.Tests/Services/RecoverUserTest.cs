using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using rc = TeamPhoenix.MusiCali.Services.RecoverUser;

namespace TeamPhoenix.MusiCali.Tests.Services
{
    [TestClass]
    public class RecoveryTest
    {
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
            UserCreation.CreateUser(userAccount, userAuth, userR, userC, userP);

            // Arrange
            string username = "username";

            // Act
            bool result = rc.EnableUser(username);

            // Assert
            Assert.IsTrue(result);
            // Add more assertions based on the expected behavior of the recovery service
        }

        [TestMethod]
        public void IsValidUsername_ShouldReturnTrueForValidUsername()
        {
            // Arrange
            string validUsername = "validusername";

            // Act
            bool result = rc.isValidUsername(validUsername);

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
            bool result = rc.ValidateOTP(OTP, storedOTP);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RecoverDisabledAccount_ShouldThrowExceptionForNonMatchingUsername()
        {
            // Arrange
            string username = "testuser";

            // Act

            // Assert
            Assert.ThrowsException<Exception>(() => rc.EnableUser(username));
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