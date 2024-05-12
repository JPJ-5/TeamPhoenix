using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class GigFindTest
    {
        [TestMethod]
        public void GigFindTest_ShouldReturnTrueForSuccessfulEmptyModel()
        {
            // Arrange
            var gigFindModel = new GigFindModel();

            // Assert
            Assert.AreEqual(string.Empty, gigFindModel.Username);
            Assert.AreEqual(default(DateTime), gigFindModel.DateOfGig);
        }
        [TestMethod]
        public void GigFindTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            var gigFindModel = new GigFindModel();

            // Act
            gigFindModel.Username = "testuser";
            DateTime testDate = DateTime.Now;
            gigFindModel.DateOfGig = testDate;

            // Assert
            Assert.AreEqual("testuser", gigFindModel.Username);
            Assert.AreEqual(testDate.Date, gigFindModel.DateOfGig.Date);
        }
    }
}
