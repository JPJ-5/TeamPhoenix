using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class LoggingLibraryTest
    {
        private readonly IConfiguration configuration;
        private LoggerService loggerService;
        private UserCreationService userCreationService;
        private RecoverUserDAO recoverUserDAO;
        private UserDeletionDAO userDeletionDAO;

        public LoggingLibraryTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            loggerService = new LoggerService(configuration);
            userCreationService = new UserCreationService(configuration);
            recoverUserDAO = new RecoverUserDAO(configuration);
            userDeletionDAO = new UserDeletionDAO(configuration);
        }

        [TestMethod]
        public void CreateLog_And_Returns_Success()
        {
            // Arrange
            string email = "test1234@example.com";
            string backupEmail = "backuestemailtry@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "testuser123";
            bool result = userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            string level = "test level";
            string category = "test category";
            string context = "test context";
            userCreationService.RegisterNormalUser(email, dateOfBirth, username, backupEmail);
            var uHash = recoverUserDAO.GetUserHash(username);
            Result logResult = loggerService.CreateLog(uHash, level, category, context);
            userDeletionDAO.DeleteProfile(username);

            Assert.IsFalse(logResult.HasError);

        }

    }
}
