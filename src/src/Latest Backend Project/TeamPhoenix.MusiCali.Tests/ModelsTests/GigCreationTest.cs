using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class GigCreationTest
    {
        [TestMethod]
        public void GigCreationTest_ShouldReturnTrueForSuccessfulEmptyModel()
        {
            // Arrange
            var gig = new GigCreationModel();

            // Assert
            Assert.AreEqual(string.Empty, gig.Username);
            Assert.AreEqual(string.Empty, gig.GigName);
            Assert.AreEqual(default(DateTime), gig.DateOfGig);
            Assert.IsFalse(gig.Visibility);
            Assert.AreEqual(string.Empty, gig.Description);
            Assert.AreEqual(string.Empty, gig.Location);
            Assert.AreEqual(string.Empty, gig.Pay);
        }
        [TestMethod]
        public void GigCreationTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            var gig = new GigCreationModel();

            // Act
            gig.Username = "testuser";
            gig.GigName = "Test Gig";
            DateTime testDate = DateTime.Now;
            gig.DateOfGig = testDate;
            gig.Visibility = true;
            gig.Description = "This is a test gig";
            gig.Location = "Test location";
            gig.Pay = "100";

            // Assert
            Assert.AreEqual("testuser", gig.Username);
            Assert.AreEqual("Test Gig", gig.GigName);
            Assert.AreEqual(testDate.Date, gig.DateOfGig.Date);
            Assert.IsTrue(gig.Visibility);
            Assert.AreEqual("This is a test gig", gig.Description);
            Assert.AreEqual("Test location", gig.Location);
            Assert.AreEqual("100", gig.Pay);
        }
    }
}
