using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class GigVisibilityTest
    {
        [TestMethod]
        public void GigVisibilityTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var visibilityModel = new GigVisibilityModel();

            // Assert
            Assert.AreEqual(string.Empty, visibilityModel.Username);
            Assert.IsFalse(visibilityModel.GigVisibility);
        }
        [TestMethod]
        public void GigVisibilityTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            string expectedUsername = "testuser";
            bool expectedGigVisibility = true;

            // Act
            var visibilityModel = new GigVisibilityModel
            {
                Username = expectedUsername,
                GigVisibility = expectedGigVisibility
            };

            // Assert
            Assert.AreEqual(expectedUsername, visibilityModel.Username);
            Assert.AreEqual(expectedGigVisibility, visibilityModel.GigVisibility);
        }
    }
}
