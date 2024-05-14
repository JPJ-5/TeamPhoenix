using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class LogFeatureTest
    {
        [TestMethod]
        public void LogFeatureTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var logFeature = new LogFeature();

            // Assert
            Assert.AreEqual(string.Empty, logFeature.UserName);
            Assert.AreEqual(string.Empty, logFeature.Feature);
        }
        [TestMethod]
        public void LogFeatureTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            string expectedUserName = "testuser";
            string expectedFeature = "Feature1";

            // Act
            var logFeature = new LogFeature
            {
                UserName = expectedUserName,
                Feature = expectedFeature
            };

            // Assert
            Assert.AreEqual(expectedUserName, logFeature.UserName);
            Assert.AreEqual(expectedFeature, logFeature.Feature);
        }
    }
}
