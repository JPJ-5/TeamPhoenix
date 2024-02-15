using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.Models
{
    [TestClass]
    public class UserRecoveryModelTest
    {
        [TestMethod]
        public void UserRecoveryModelTest_ShouldReturnTrueForSuccessfulModel()
        {
            // Arrange
            string username = "TestUser";
            string question = "Test Question";
            string answer = "Test Answer";

            //Act
            UserRecovery userRecovery = new UserRecovery(username, question, answer);

            // Assert
            Assert.IsNotNull(userRecovery);
            Assert.AreEqual(username, userRecovery.Username);
            Assert.AreEqual(question, userRecovery.Question);
            Assert.AreEqual(answer, userRecovery.Answer);
        }
    }
}
