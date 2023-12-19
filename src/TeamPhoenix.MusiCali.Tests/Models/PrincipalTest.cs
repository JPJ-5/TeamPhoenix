using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.Models
{
    [TestClass]
    public class PrincipalTest
    {
        [TestMethod]
        public void PrincipalTest_ShouldReturnTrueForSuccessfulModel()
        {
            // Arrange
            Dictionary<string, string> testClaims = new Dictionary<string, string>();
            string testUsername = "testUser";

            //Act
            Principal principal = new Principal(testUsername, testClaims);

            // Assert
            Assert.IsNotNull(principal);
            Assert.AreEqual(testUsername, principal.Username);
            Assert.AreEqual(testClaims, principal.Claims);
        }
    }
}