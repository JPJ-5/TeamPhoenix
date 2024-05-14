using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class AuthResultTest
    {
        [TestMethod]
        public void AuthResult_ShouldReturnTrueForSuccessfulModel()
        {
            // Arrange
            Dictionary<string, string> testClaims = new Dictionary<string, string>();
            UserAuthN userAuth = new UserAuthN("TestUser", "TestOTP", DateTime.UtcNow, "TestSalt");
            UserAccount userAccount = new UserAccount("TestUser", "TestSalt", "testHash", "testEmail@example.com");
            UserClaims userClaims = new UserClaims("TestUser", testClaims);

            //Act
            AuthResult authResult = new AuthResult(userAuth, userAccount, userClaims);

            // Assert
            Assert.IsNotNull(authResult);
            Assert.AreEqual(userAuth, authResult.userA);
            Assert.AreEqual(userAccount, authResult.userAcc);
            Assert.AreEqual(userClaims, authResult.userC);
        }
    }
}
