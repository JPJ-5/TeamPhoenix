using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class LoginModelTest
    {
        [TestMethod]
        public void LoginModelTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var loginModel = new LoginModel();

            // Assert
            Assert.AreEqual(string.Empty, loginModel.Username);
            Assert.AreEqual(string.Empty, loginModel.Otp);
        }
        [TestMethod]
        public void LoginModelTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            string expectedUsername = "testuser";
            string expectedOtp = "123456";

            // Act
            var loginModel = new LoginModel
            {
                Username = expectedUsername,
                Otp = expectedOtp
            };

            // Assert
            Assert.AreEqual(expectedUsername, loginModel.Username);
            Assert.AreEqual(expectedOtp, loginModel.Otp);
        }
    }
}
