using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class GigViewTest
    {
        [TestMethod]
        public void GigViewTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            string expectedUsername = "testuser";
            string expectedGigName = "Test Gig";
            DateTime expectedDateOfGig = DateTime.Now;
            bool expectedVisibility = true;
            string expectedDescription = "Test Description";
            string expectedLocation = "Test Location";
            string expectedPay = "100";

            // Act
            var gigView = new GigView(
                expectedUsername,
                expectedGigName,
                expectedDateOfGig,
                expectedVisibility,
                expectedDescription,
                expectedLocation,
                expectedPay);

            // Assert
            Assert.AreEqual(expectedUsername, gigView.Username);
            Assert.AreEqual(expectedGigName, gigView.GigName);
            Assert.AreEqual(expectedDateOfGig, gigView.DateOfGig);
            Assert.AreEqual(expectedVisibility, gigView.Visibility);
            Assert.AreEqual(expectedDescription, gigView.Description);
            Assert.AreEqual(expectedLocation, gigView.Location);
            Assert.AreEqual(expectedPay, gigView.Pay);
        }
    }
}
