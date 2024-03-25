using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;
using uc = TeamPhoenix.MusiCali.Services.UserCreation;

namespace TeamPhoenix.MusiCali.Tests.Services
{
    [TestClass]
    public class UserCreationTest
    {
        [TestMethod]
        public void RegisterUser_ShouldReturnTrueForValidRegistration()
        {
            // Arrange
            string email = "test@example.com";
            string backupEmail = "backupTest@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";

            // Act
            bool result = uc.RegisterNormalUser(email, dateOfBirth, username, backupEmail);

            // Assert
            Assert.IsTrue(result);
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
            bool result = uc.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            // Assert
            Assert.ThrowsException<ArgumentException>(() => uc.RegisterNormalUser(email, dateOfBirth, username, backupEmail));
        }

        [TestMethod]
        public void RegisterUser_ShouldThrowArgumentExceptionForInvalidDateOfBirth()
        {
            string email = "test3@example.com";
            string backupEmail = "backupTest3@example.com";
            DateTime dateOfBirth = new DateTime(1669, 1, 1);
            string username = "testuser3";

            // Act
            bool result = uc.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            // Assert
            Assert.ThrowsException<ArgumentException>(() => uc.RegisterNormalUser(email, dateOfBirth, username, backupEmail));
        }
        [TestMethod]
        public void RegisterUser_ShouldThrowArgumentExceptionForInvalidUsername()
        {
            string email = "test4@example.com";
            string backupEmail = "backupTest4@example.com";
            DateTime dateOfBirth = new DateTime(1800, 1, 1);
            string username = "testuser4!"; //should return an invalid username

            // Act
            bool result = uc.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            // Assert
            Assert.ThrowsException<ArgumentException>(() => uc.RegisterNormalUser(email, dateOfBirth, username, backupEmail));
        }

        // Add more test methods for other scenarios.

        [TestCleanup]
        public void Cleanup()
        {
            Tester.DeleteAllRows("UserAuthN");
            Tester.DeleteAllRows("UserProfile");
            Tester.DeleteAllRows("UserRecovery");
            Tester.DeleteAllRows("UserClaims");
            Tester.DeleteAllRows("UserAccount");
        }
    }
}