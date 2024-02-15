using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.Models
{
    [TestClass]
    public class UserAccountTest
    {
        [TestMethod]
        public void UserAccountTest_ShouldReturnTrueForSuccessfulModel()
        {
            // Arrange
            string username = "TestUser";
            string salt = "TestSalt";
            string hash = "testHash";
            string email = "testEmail@example.com";

            //Act
            UserAccount userAccount = new UserAccount(username, salt, hash, email);

            // Assert
            Assert.IsNotNull(userAccount);
            Assert.AreEqual(username, userAccount.Username);
            Assert.AreEqual(salt, userAccount.Salt);
            Assert.AreEqual(hash, userAccount.UserHash);
            Assert.AreEqual(email, userAccount.Email);
        }
    }
}
