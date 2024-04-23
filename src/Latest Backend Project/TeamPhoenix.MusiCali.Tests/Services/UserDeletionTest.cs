using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Tests.Services
{
    [TestClass]
    public class UserDeletionTest
    {
        UserCreationService userCreationService = new UserCreationService();
        UserDeletionService userDeletionService = new UserDeletionService();
        [TestMethod]
        public void DeleteUser_ShouldReturnTrueAndDeleteUser()
        {
            string email = "deletetest@example.com";
            string backupEmail = "backupdeleteTest@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "deletetestuser";

            bool createRes = userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            Assert.IsTrue(createRes);

            bool deleteRes = userDeletionService.DeleteUser(username);

            Assert.IsTrue(deleteRes);

        }

        public void DeleteUser_ShouldReturnFalseForInvalidUsername()
        {
            string username = "deletetestuser";

            bool deleteRes = userDeletionService.DeleteUser(username);

            Assert.IsFalse(deleteRes);

        }
    }
}
