using TeamPhoenix.MusiCali.Services;
namespace TeamPhoenix.MusiCali.Tests.Services
{
    [TestClass]
    public class ModifyUserTest
    {
        ModifyUserService modifyUserService = new ModifyUserService();

        [TestMethod]
        public void IsNameValid_ShouldReturnTrueForValidName()
        {
            // Arrange
            string validName = "John";

            // Act
            bool result = modifyUserService.isNameValid(validName);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsNameValid_ShouldReturnFalseForInvalidName()
        {
            // Arrange
            string invalidName = "John123";

            // Act
            bool result = modifyUserService.isNameValid(invalidName);

            // Assert
            Assert.IsFalse(result);
        }
        /*
        [TestMethod]
        public void IsDateOfBirthValid_ShouldReturnTrueForValidDateOfBirth()
        {
            // Arrange
            DateTime validDateOfBirth = new DateTime(1990, 5, 15);

            // Act
            bool result = mu.isDateOfBirth(validDateOfBirth);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsDateOfBirthValid_ShouldReturnFalseForInvalidDateOfBirth()
        {
            // Arrange
            DateTime invalidDateOfBirth = new DateTime(1800, 1, 1);

            // Act
            bool result = mu.isDateOfBirth(invalidDateOfBirth);

            // Assert
            Assert.IsFalse(result);
        }
        */
    }
}