using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class GigToViewTest
    {
        [TestMethod]
        public void GigToViewTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var viewModel = new GigToViewModel();

            // Assert
            Assert.AreEqual(string.Empty, viewModel.Username);
            Assert.AreEqual(default(DateTime), viewModel.DateOfGig);
            Assert.AreEqual(string.Empty, viewModel.UsernameOwner);
        }
        [TestMethod]
        public void GigToViewTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            string expectedUsername = "testuser";
            DateTime expectedDateOfGig = DateTime.Now;
            string expectedUsernameOwner = "owneruser";

            // Act
            var viewModel = new GigToViewModel
            {
                Username = expectedUsername,
                DateOfGig = expectedDateOfGig,
                UsernameOwner = expectedUsernameOwner
            };

            // Assert
            Assert.AreEqual(expectedUsername, viewModel.Username);
            Assert.AreEqual(expectedDateOfGig, viewModel.DateOfGig);
            Assert.AreEqual(expectedUsernameOwner, viewModel.UsernameOwner);
        }
    }
}
