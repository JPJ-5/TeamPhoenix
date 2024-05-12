using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class GigUpdateTest
    {
        [TestMethod]
        public void GigUpdateTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var updateModel = new GigUpdateModel();

            // Assert
            Assert.AreEqual(default(DateTime), updateModel.DateOfGigOriginal);
            Assert.AreEqual(string.Empty, updateModel.Username);
            Assert.AreEqual(string.Empty, updateModel.GigName);
            Assert.AreEqual(default(DateTime), updateModel.DateOfGig);
            Assert.IsFalse(updateModel.Visibility);
            Assert.AreEqual(string.Empty, updateModel.Description);
            Assert.AreEqual(string.Empty, updateModel.Location);
            Assert.AreEqual(string.Empty, updateModel.Pay);
        }
        [TestMethod]
        public void GigUpdateTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            DateTime expectedDateOfGigOriginal = DateTime.Now;
            string expectedUsername = "testuser";
            string expectedGigName = "Test Gig";
            DateTime expectedDateOfGig = DateTime.Now.AddDays(7);
            bool expectedVisibility = true;
            string expectedDescription = "Test Description";
            string expectedLocation = "Test Location";
            string expectedPay = "100";

            // Act
            var updateModel = new GigUpdateModel
            {
                DateOfGigOriginal = expectedDateOfGigOriginal,
                Username = expectedUsername,
                GigName = expectedGigName,
                DateOfGig = expectedDateOfGig,
                Visibility = expectedVisibility,
                Description = expectedDescription,
                Location = expectedLocation,
                Pay = expectedPay
            };

            // Assert
            Assert.AreEqual(expectedDateOfGigOriginal, updateModel.DateOfGigOriginal);
            Assert.AreEqual(expectedUsername, updateModel.Username);
            Assert.AreEqual(expectedGigName, updateModel.GigName);
            Assert.AreEqual(expectedDateOfGig, updateModel.DateOfGig);
            Assert.AreEqual(expectedVisibility, updateModel.Visibility);
            Assert.AreEqual(expectedDescription, updateModel.Description);
            Assert.AreEqual(expectedLocation, updateModel.Location);
            Assert.AreEqual(expectedPay, updateModel.Pay);
        }
    }
}
