using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class GigSummaryListTest
    {
        [TestMethod]
        public void GigSummaryListTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var gigSet = new GigSet();

            // Assert
            Assert.IsNotNull(gigSet.GigSummaries);
            Assert.AreEqual(0, gigSet.GigSummaries.Count);
        }
        [TestMethod]
        public void GigSummaryListTest_ShouldReturnTrueForAddingSummaryToList()
        {
            // Arrange
            var gigSet = new GigSet();
            var gigSummary = new GigSummary("testuser", "Test Gig", DateTime.Now, "Test Location", "100", "Test Description", 123, true);

            // Act
            gigSet.GigSummaries?.Add(gigSummary);

            // Assert
            Assert.AreEqual(1, gigSet.GigSummaries?.Count);
            Assert.IsTrue(gigSet.GigSummaries?.Contains(gigSummary));
        }

        [TestMethod]
        public void GigSummaryListTest_ShouldReturnTrueForRemovingSummaryToList()
        {
            // Arrange
            var gigSet = new GigSet();
            var gigSummary = new GigSummary("testuser", "Test Gig", DateTime.Now, "Test Location", "100", "Test Description", 123, true);
            gigSet.GigSummaries?.Add(gigSummary);

            // Act
            gigSet.GigSummaries?.Remove(gigSummary);

            // Assert
            Assert.AreEqual(0, gigSet.GigSummaries?.Count);
            Assert.IsFalse(gigSet.GigSummaries?.Contains(gigSummary));
        }
    }
}
