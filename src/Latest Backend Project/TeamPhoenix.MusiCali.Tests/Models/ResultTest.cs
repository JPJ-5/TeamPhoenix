using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.Models
{
    [TestClass]
    public class ResultTest
    {
        [TestMethod]
        public void ResultTest_ShouldReturnTrueForSuccessfulModel()
        {
            // Arrange
            string testMessage = "test message";
            bool testSuccess = true;

            //Act
            Result result = new Result(testMessage, testSuccess);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(testMessage, result.ErrorMessage);
            Assert.AreEqual(testSuccess, result.Success);
        }
    }
}
