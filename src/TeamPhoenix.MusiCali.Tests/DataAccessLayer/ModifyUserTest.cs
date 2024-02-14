using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using dao = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;
using uc = TeamPhoenix.MusiCali.Services.UserCreation;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;
using System.Diagnostics.Metrics;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{
    [TestClass]
    public class ModifyUserTest
    {
        private const string ConnectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

        [TestMethod]
        public void NewUpdateProfile_ShouldUpdateUserProfile()
        {
            // Arrange
            DateTime dob = new DateTime(2001, 01, 26);
            string username = "Julie0126";
            uc.RegisterUser("joshuareyes@gmail.com", dob, username, "Julie", "Reyes", "What is your pets name", "Ace");
            UserProfile userProfile = new UserProfile(username, "UpdatedFirstName", "UpdatedLastName", dob);

            // Insert the user profile into the database
            dao.UpdateProfile(userProfile);

            // Instantiate the ModifyUser class
            ModifyUser modifyUser = new ModifyUser();

            // Act - Retrieve the profile using the GetProfile method
            UserProfile retrievedProfile = modifyUser.GetProfile(username);

            // Assert - Check if the profile is retrieved correctly
            Assert.AreEqual(userProfile.FirstName, retrievedProfile.FirstName);
            Assert.AreEqual(userProfile.LastName, retrievedProfile.LastName);
            Assert.AreEqual(userProfile.DOB, retrievedProfile.DOB);
        }

        [TestMethod]
        public void ModifyProfile_Should_Update_User_Profile()
        {
            // Arrange
            var modifyUser = new ModifyUser();
            string username = "testuser10";
            string firstName = "John";
            string lastName = "Doe";

            // Act
            var result = modifyUser.ModifyProfile(username, firstName, lastName);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DeleteProfile_ShouldDeleteUserProfileAndUserAccount()
        {
            // Arrange
            ModifyUser modifyUser = new ModifyUser();
            string username = "testuser01";

            // Act
            bool result = modifyUser.DeleteProfile(username);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UpdateClaims_Should_Update_Claims_For_Existing_User()
        {
            // Arrange
            var modifyUser = new ModifyUser();
            string username = "testuser10";
            Dictionary<string, string> updatedClaims = new Dictionary<string, string>
        {
            { "UserRole", "admin" }
        };

            // Act
            var result = modifyUser.UpdateClaims(username, updatedClaims);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UpdateProfile_ShouldThrowExceptionForInvalidUsername()
        {
            // Arrange
            DateTime dob = new DateTime(2000, 01, 01);
            uc.RegisterUser("joshuareyes@gmail.com", dob, "Julie0126", "Julie", "Reyes", "What is your pets name", "Ace");
            UserProfile userProfile = new UserProfile("nonexistentuser", "Test", "User", dob);

            // Act and Assert
            Assert.ThrowsException<Exception>(() => dao.UpdateProfile(userProfile));
        }

        [TestMethod]
        public void UpdateProfile_ShouldThrowExceptionForNullUserProfile()
        {
            // Act and Assert
            DateTime dob = new DateTime(2000, 01, 01);
            uc.RegisterUser("joshuareyes@gmail.com", dob, "Julie0126", "Julie", "Reyes", "What is your pets name", "Ace");
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