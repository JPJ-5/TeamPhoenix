using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.Logging;
using TeamPhoenix.MusiCali.Services;
namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class ModifyUserServiceTest
    {
        private readonly IConfiguration configuration;
        private ModifyUserService modifyUserService;
        private UserCreationService userCreationService;
        private UserDeletionService userDeletionService;
        public ModifyUserServiceTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            modifyUserService = new ModifyUserService(configuration);
            userCreationService = new UserCreationService(configuration);
            userDeletionService = new UserDeletionService(configuration);
        }

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
    }
}