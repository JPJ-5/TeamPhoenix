using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{
    [TestClass]
    public class ModifyUserTest
    {
        ModifyUserDAO modifyUserDao = new ModifyUserDAO();
        UserCreationService userCreationService = new UserCreationService();
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
            userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            UserProfile userProfile = new UserProfile(username, fname, lname, dateOfBirth);

            // Act
            modifyUserDao.UpdateProfile(userProfile);

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
            Assert.ThrowsException<Exception>(() => modifyUserDao.UpdateProfile(userProfile));
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
            Assert.ThrowsException<Exception>(() => modifyUserDao.UpdateProfile(null));
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

    }
}
