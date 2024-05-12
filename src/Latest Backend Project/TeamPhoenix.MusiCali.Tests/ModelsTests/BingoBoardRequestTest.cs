using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class BingoBoardRequestTest
    {
        [TestMethod]
        public void BingoBoardRequest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var request = new BingoBoardRequest();

            // Assert
            Assert.AreEqual(10, request.NumberOfGigs);
            Assert.AreEqual("", request.Username);
            Assert.AreEqual(0, request.Offset);
        }
        [TestMethod]
        public void BingoBoardRequest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            int expectedNumberOfGigs = 5;
            string expectedUsername = "testuser";
            int expectedOffset = 2;

            // Act
            var request = new BingoBoardRequest();
            request.NumberOfGigs = expectedNumberOfGigs;
            request.Username = expectedUsername;
            request.Offset = expectedOffset;


            // Assert
            Assert.AreEqual(expectedNumberOfGigs, request.NumberOfGigs);
            Assert.AreEqual(expectedUsername, request.Username);
            Assert.AreEqual(expectedOffset, request.Offset);
        }
    }
}
