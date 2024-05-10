using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;

namespace Teamphoenix.Musicali.Tests
{
    [TestClass]
    public class ModifyUserDaoTest
    {
        private readonly IConfiguration configuration;
        private ModifyUserDAO modifyUserDAO;
        private UserCreationService userCreationService;
        private UserDeletionService userDeletionService;

        public ModifyUserDaoTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            modifyUserDAO = new ModifyUserDAO(configuration);
            userCreationService = new UserCreationService(configuration);
            userDeletionService = new UserDeletionService(configuration);
        }

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
            userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            UserProfile userProfile = new UserProfile(updatedUsername, fname, lname, dateOfBirth);

            // Act
            modifyUserDAO.UpdateProfile(userProfile);

            //getuserinformation returnes an unclassed object
            
            object updatedProfile = new {
                FirstName = "",
                LastName = "",
                DateOfBirth = "",
                Email = "",
                UserStatus = "",
                UserRole =""
            };
            
            // Assert - Check if the profile is updated
            //updatedProfile = dao.GetUserInformation(username);
            Assert.AreEqual(userProfile.FirstName, fname);
            Assert.AreEqual(userProfile.LastName, lname);
            Assert.AreEqual(userProfile.DOB, dateOfBirth);
            userDeletionService.DeleteUser(updatedUsername);
        }

        [TestMethod]
        public void UpdateProfile_ShouldThrowExceptionForInvalidUsername()
        {
            // Arrange
            string email = "test@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";
            string backupEmail = "";
            userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            UserProfile userProfile = new UserProfile("nonexistentuser", "Test", "User", dateOfBirth);

            // Act and Assert
            Assert.ThrowsException<Exception>(() => modifyUserDAO.UpdateProfile(userProfile));
            userDeletionService.DeleteUser(username);
        }

        [TestMethod]
        public void UpdateProfile_ShouldThrowExceptionForNullUserProfile()
        {
            // Act and Assert
            string email = "test@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser";
            string backupEmail = "";
            userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            Assert.ThrowsException<Exception>(() => modifyUserDAO.UpdateProfile(null!));
            userDeletionService.DeleteUser(username);
        }

    }
}
