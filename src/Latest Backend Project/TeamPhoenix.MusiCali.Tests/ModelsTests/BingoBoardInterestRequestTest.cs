using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class BingoBoardInterestRequestTest
    {
        [TestMethod]
        public void BingoBoardInterestRequestTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var request = new BingoBoardInterestRequest();

            // Assert
            Assert.AreEqual("", request.username);
            Assert.AreEqual(0, request.gigID);
        }
        [TestMethod]
        public void BingoBoardInterestRequestTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            string expectedUsername = "testuser";
            int expectedGigID = 123;

            // Act
            var request = new BingoBoardInterestRequest();
            request.username = "testuser";
            request.gigID = 123;

            // Assert
            Assert.AreEqual(expectedUsername, request.username);
            Assert.AreEqual(expectedGigID, request.gigID);
        }
    }
}
