using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using dao = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;
using uC = TeamPhoenix.MusiCali.Services.UserCreation;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{
    [TestClass]
    public class ModifyUserTest
    {
        private const string ConnectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

        [TestMethod]
        public void UpdateProfile_ShouldUpdateUserProfile()
        {
            // Arrange
            string email = "test@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";
            string updatedUsername = "testuser1";
            string fname = "John";
            string lname = "Doe";
            string backupEmail = "";
            uC.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            UserProfile userProfile = new UserProfile(username, fname, lname, dateOfBirth);

            // Act
            dao.UpdateProfile(userProfile);

            //getuserinformation returnes an unclassed object
            /*
             * var updatedProfile = new {
                FirstName = "",
                LastName = "",
                DateOfBirth = "",
                Email = "",
                UserStatus = "",
                UserRole =""
            };
            */
            // Assert - Check if the profile is updated
            var updatedProfile = dao.GetUserInformation(username);
            Assert.AreEqual(userProfile.FirstName, updatedProfile.FirstName);
            Assert.AreEqual(userProfile.LastName, updatedProfile.LastName);
            Assert.AreEqual(userProfile.DOB, updatedProfile.DOB);
        }

        [TestMethod]
        public void UpdateProfile_ShouldThrowExceptionForInvalidUsername()
        {
            // Arrange
            string email = "test@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";
            string backupEmail = "";
            uC.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            UserProfile userProfile = new UserProfile("nonexistentuser", "Test", "User", dateOfBirth);

            // Act and Assert
            Assert.ThrowsException<Exception>(() => dao.UpdateProfile(userProfile));
        }

        [TestMethod]
        public void UpdateProfile_ShouldThrowExceptionForNullUserProfile()
        {
            // Act and Assert
            string email = "test@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";
            string backupEmail = "";
            uC.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            Assert.ThrowsException<Exception>(() => dao.UpdateProfile(null));
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

    }
}
