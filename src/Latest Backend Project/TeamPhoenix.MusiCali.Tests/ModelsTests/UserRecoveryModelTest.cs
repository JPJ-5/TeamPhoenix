using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class UserRecoveryModelTest
    {
        [TestMethod]
        public void UserRecoveryModelTest_ShouldReturnTrueForSuccessfulModel()
        {
            // Arrange
            string username = "TestUser";
            string backupEmail = "TestEmail";

            //Act
            UserRecovery userRecovery = new UserRecovery(username, backupEmail);

            // Assert
            Assert.IsNotNull(userRecovery);
            Assert.AreEqual(username, userRecovery.Username);
            Assert.AreEqual(backupEmail, userRecovery.backupEmail);
        }
    }
}
