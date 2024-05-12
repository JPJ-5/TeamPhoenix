using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class LoginTokenTest
    {
        [TestMethod]
        public void LoginTokenTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var loginTokens = new LoginTokens();

            // Assert
            Assert.IsNull(loginTokens.IdToken);
            Assert.IsNull(loginTokens.AccToken);
            Assert.IsFalse(loginTokens.Success);
        }
        [TestMethod]
        public void LoginTokenTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            string expectedIdToken = "idToken123";
            string expectedAccToken = "accToken456";
            bool expectedSuccess = true;

            // Act
            var loginTokens = new LoginTokens
            {
                IdToken = expectedIdToken,
                AccToken = expectedAccToken,
                Success = expectedSuccess
            };

            // Assert
            Assert.AreEqual(expectedIdToken, loginTokens.IdToken);
            Assert.AreEqual(expectedAccToken, loginTokens.AccToken);
            Assert.AreEqual(expectedSuccess, loginTokens.Success);
        }
    }
}
