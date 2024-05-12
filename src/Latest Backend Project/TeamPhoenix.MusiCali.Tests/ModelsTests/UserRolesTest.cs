using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class UserRolesTest
    {
        [TestMethod]
        public void UserRolesTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var userRoles = new UserRoles();

            // Assert
            Assert.AreEqual(string.Empty, userRoles.UserRole);
        }
        [TestMethod]
        public void UserRolesTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            string expectedUserRole = "Admin";

            // Act
            var userRoles = new UserRoles
            {
                UserRole = expectedUserRole
            };

            // Assert
            Assert.AreEqual(expectedUserRole, userRoles.UserRole);
        }
    }
}
