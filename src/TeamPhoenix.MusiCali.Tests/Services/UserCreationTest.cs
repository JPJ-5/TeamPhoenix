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
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";
            string fname = "John";
            string lname = "Doe";
            string q = "Security Question";
            string a = "a";

            // Act
            bool result = uc.RegisterUser(email, dateOfBirth, username, fname, lname, q, a);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RegisterUser_ShouldThrowArgumentExceptionForInvalidEmail()
        {
            // Arrange
            string email = "test2@example.com";
            DateTime dateOfBirth = new DateTime(1950, 1, 1);
            string username = "testuser2";
            string fname = "John";
            string lname = "Doe";
            string q = "Security Question";
            string a = "a";

            // Act

            // Assert
            Assert.ThrowsException<ArgumentException>(() => uc.RegisterUser(email, dateOfBirth, username, fname, lname, q, a));
        }

        [TestMethod]
        public void RegisterUser_ShouldThrowArgumentExceptionForInvalidDateOfBirth()
        {
            string email = "test3@example.com";
            DateTime dateOfBirth = new DateTime(1669, 1, 1);
            string username = "testuser3";
            string fname = "John";
            string lname = "Doe";
            string q = "Security Question";
            string a = "a";

            // Act

            // Assert
            Assert.ThrowsException<ArgumentException>(() => uc.RegisterUser(email, dateOfBirth, username, fname, lname, q, a));
        }
        [TestMethod]
        public void RegisterUser_ShouldThrowArgumentExceptionForInvalidUsername()
        {
            string email = "test4@example.com";
            DateTime dateOfBirth = new DateTime(1800, 1, 1);
            string username = "testuser4!"; //should return an invalid username
            string fname = "John";
            string lname = "Doe";
            string q = "Security Question";
            string a = "a";

            // Act

            // Assert
            Assert.ThrowsException<ArgumentException>(() => uc.RegisterUser(email, dateOfBirth, username, fname, lname, q, a));
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