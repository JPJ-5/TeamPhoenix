using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.Models
{
    [TestClass]
    public class UserClaimsTest
    {
        [TestMethod]
        public void UserClaimsTest_ShouldReturnTrueForSuccessfulModel()
        {
            // Arrange
            string username = "TestUser";
            Dictionary<string, string> testClaims = new Dictionary<string, string>();

            //Act
            UserClaims userClaim = new UserClaims(username, testClaims);

            // Assert
            Assert.IsNotNull(userClaim);
            Assert.AreEqual(username, userClaim.Username);
            Assert.AreEqual(testClaims, userClaim.Claims);
        }
    }
}
