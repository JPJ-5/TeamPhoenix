﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using dao = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{
    [TestClass]
    public class RecoverUserTest
    {
        [TestMethod]
        public void GetUserRecovery_ShouldReturnUserRecoveryObject()
        {
            // Arrange
            Dictionary<string, string> claims = new Dictionary<string, string>
            {
                {"UserRole", "User"}
            };
            DateTime dob = new DateTime(2001, 01, 26);
            UserAccount userAccount = new UserAccount("username", "testsalt", "fakehash", "email");
            UserAuthN userAuth = new UserAuthN("username", "testotp", DateTime.Now, "testsalt");
            UserRecovery userR = new UserRecovery("username", "security question", "answer to question");
            UserClaims userC = new UserClaims("username", claims);

            UserProfile userP = new UserProfile("username", "prof", "vong", dob);

            // Act
            UserCreation.CreateUser(userAccount, userAuth, userR, userC, userP);

            // Act
            UserRecovery result = dao.GetUserRecovery("username");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("username", result.Username);
            // Add more assertions based on the expected data in the UserRecovery object
        }

        [TestMethod]
        public void UpdateUserRecovery_ShouldReturnTrueForSuccessfulUpdate()
        {
            Dictionary<string, string> claims = new Dictionary<string, string>
            {
                {"UserRole", "User"}
            };
            DateTime dob = new DateTime(2001, 01, 26);
            UserAccount userAccount = new UserAccount("username", "testsalt", "fakehash", "email");
            UserAuthN userAuth = new UserAuthN("username", "testotp", DateTime.Now, "testsalt");
            UserRecovery userR = new UserRecovery("username", "security question", "answer to question");
            UserClaims userC = new UserClaims("username", claims);

            UserProfile userP = new UserProfile("username", "prof", "vong", dob);

            // Act
            UserCreation.CreateUser(userAccount, userAuth, userR, userC, userP);
            UserRecovery userRecovery = new UserRecovery("username", "Security Question", "Security Answer");
            userRecovery.Success = true;

            // Act
            bool result = dao.updateUserR(userRecovery);

            // Assert
            Assert.IsTrue(result);
        }

        // Add more test methods for other scenarios

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
