using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class BingoBoardInterestMessageTest
    {
        [TestMethod]
        public void BingoBoardInterestMessageTest_ShouldReturnTrueForSuccessfulEmptyModel()
        {
            // Arrange
            string expectedMsg = "Success";
            bool expectedSuccess = true;

            // Act
            var message = new BingoBoardInterestMessage(expectedMsg, expectedSuccess);

            // Assert
            Assert.AreEqual(expectedMsg, message.returnMsg);
            Assert.AreEqual(expectedSuccess, message.returnSuccess);
        }
    }
}
