using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using dao = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;
using uc = TeamPhoenix.MusiCali.Services.UserCreation;
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
            DateTime dob = new DateTime(2001, 01, 26);
            uc.RegisterUser("joshuareyes@gmail.com", dob, "Julie0126", "Julie", "Reyes", "What is your pets name", "Ace");
            UserProfile userProfile = new UserProfile("julie0126", "Julie", "Reyes", dob);

            // Act
            dao.UpdateProfile(userProfile);

            // Assert - Check if the profile is updated
            UserProfile updatedProfile = dao.GetUserProfile("julie0126");
            Assert.AreEqual(userProfile.FirstName, updatedProfile.FirstName);
            Assert.AreEqual(userProfile.LastName, updatedProfile.LastName);
            Assert.AreEqual(userProfile.DOB, updatedProfile.DOB);
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
