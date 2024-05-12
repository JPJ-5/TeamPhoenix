using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class GigSummaryTest
    {
        [TestMethod]
        public void GigSummaryTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            string expectedUsername = "testuser";
            string expectedGigName = "Test Gig";
            DateTime expectedDateOfGig = DateTime.Now;
            string expectedLocation = "Test Location";
            string expectedPay = "100";
            string expectedDescription = "Test Description";
            int expectedGigID = 123;
            bool expectedIsAlreadyInterested = true;

            // Act
            var gigSummary = new GigSummary(
                expectedUsername,
                expectedGigName,
                expectedDateOfGig,
                expectedLocation,
                expectedPay,
                expectedDescription,
                expectedGigID,
                expectedIsAlreadyInterested);

            // Assert
            Assert.AreEqual(expectedUsername, gigSummary.Username);
            Assert.AreEqual(expectedGigName, gigSummary.GigName);
            Assert.AreEqual(expectedDateOfGig, gigSummary.DateOfGig);
            Assert.AreEqual(expectedLocation, gigSummary.Location);
            Assert.AreEqual(expectedPay, gigSummary.Pay);
            Assert.AreEqual(expectedDescription, gigSummary.Description);
            Assert.AreEqual(expectedGigID, gigSummary.gigID);
            Assert.AreEqual(expectedIsAlreadyInterested, gigSummary.isAlreadyInterested);
        }
    }
}
