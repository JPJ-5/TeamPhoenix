using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;
using uC = TeamPhoenix.MusiCali.Services.UserCreation;
using uD = TeamPhoenix.MusiCali.Services.UserDeletion;

namespace TeamPhoenix.MusiCali.Tests.Services
{
    [TestClass]
    public class UserDeletionTest
    {
        [TestMethod]
        public void DeleteUser_ShouldReturnTrue()
        {
            string email = "deletetest@example.com";
            string backupEmail = "backupdeleteTest@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "deletetestuser";

            bool createRes = uC.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            Assert.IsTrue(createRes);

            Result deleteRes = uD.DeleteAccount(username);

        }
    }
}
