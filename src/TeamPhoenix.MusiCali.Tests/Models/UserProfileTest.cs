using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.Models
{
    [TestClass]
    public class UserProfileTest
    {
        [TestMethod]
        public void UserProfileTest_ShouldReturnTrueForSuccessfulModel()
        {
            // Arrange
            string username = "TestUser";
            string firstName = "TestFirst";
            string lastName = "TestLast";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);

            //Act
            UserProfile userProfile = new UserProfile(username, firstName, lastName, dateOfBirth);

            // Assert
            Assert.IsNotNull(userProfile);
            Assert.AreEqual(username, userProfile.Username);
            Assert.AreEqual(firstName, userProfile.FirstName);
            Assert.AreEqual(lastName, userProfile.LastName);
            Assert.AreEqual(dateOfBirth, userProfile.DOB);
        }
    }
}
