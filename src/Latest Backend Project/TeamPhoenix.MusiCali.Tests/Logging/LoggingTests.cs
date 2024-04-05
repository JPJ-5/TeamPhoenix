using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using uC = TeamPhoenix.MusiCali.Services.UserCreation;
using Log = TeamPhoenix.MusiCali.Logging.Logger;

namespace TeamPhoenix.MusiCali.Logging
{
    [TestClass]
    public class LoggingLibraryTests
    {
        private const string TestConnectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

        [TestMethod]
        public void CreateLog_And_Returns_Success()
        {
            string hash = "testHash";
            string level = "test level";
            string category = "test category";
            string context = "test context";
            Result logResult = Log.CreateLog(hash, level, category, context);

            Assert.IsFalse(logResult.HasError);

        }

        /*
        [TestMethod]
        public void CreateUser_ValidUser_ReturnsSuccessResult()
        {
            // Arrange
            var dataAccessObject = new SqlDAO();//DataAccessObject(TestConnectionString);
        
            // Act
            var result = uC.RegisterNormalUser("john.doe@email.com", new DateTime(1990, 1, 1), "johndoe", "johndoe2@email.com");

            // Assert
            Assert.IsTrue(result);//(result.hasError);
            //Assert.IsNull(result.errorMessage);
        }

        [TestMethod]
        public void CreateUser_InvalidUsername_ReturnsErrorResult()
        {
            // Arrange
            var dataAccessObject = new SqlDAO();//DataAccessObject(TestConnectionString);

            // Act
            var result = uC.RegisterNormalUser("john.doe@email.com", new DateTime(1990, 1, 1), "inv@lid username", "johndoe2@email.com");

            // Assert
            Assert.IsFalse(result);
            //Assert.IsTrue(result.hasError);
            //Assert.IsNotNull(result.errorMessage);
            //Assert.AreEqual("Invalid username. Username must be between 6 and 30 characters, and cannot contain spaces.", result.errorMessage);
        }

        [TestMethod]
        public async Task SaveLogAsync_ValidLogEntry_SavesLogSuccessfully()
        {
            // Arrange
            var dataAccessObject = new SqlDAO();//DataAccessObject(TestConnectionString);
            string logMessage = "Test log message";
            string logLevel = "LogLevel.Info";
            string logCategory = "LogCategory.Business";

            // Act
            //await dataAccessObject.SaveLogAsync(logMessage, logLevel, logCategory);
            Result res = Log.CreateLog("testHash", logMessage, logLevel, logCategory);

            // Assert
            Assert.IsTrue(res.HasError);
            //Query Database for this
        }

        [TestMethod]
        public void CreateUser_InvalidEmail_ReturnsErrorResult()
        {
            // Arrange
            var dataAccessObject = new DataAccessObject(TestConnectionString);
        
            // Act
            var result = dataAccessObject.createUser("John", "Doe", "johndoe", "invalid.email", new DateTime(1990, 1, 1), "SecureP@ssword");

            // Assert
            Assert.IsTrue(result.hasError);
            Assert.IsNotNull(result.errorMessage);
            Assert.AreEqual("Invalid email format.", result.errorMessage);
        }

        [TestMethod]
        public void CreateUser_InvalidDateOfBirth_ReturnsErrorResult()
        {
            // Arrange
            var dataAccessObject = new DataAccessObject(TestConnectionString);
        
            // Act
            var result = dataAccessObject.createUser("John", "Doe", "johndoe", "john.doe@email.com", DateTime.Now.AddYears(1), "SecureP@ssword");

            // Assert
            Assert.IsTrue(result.hasError);
            Assert.IsNotNull(result.errorMessage);
            Assert.AreEqual("Invalid date of birth. It should be between January 1st, 1970, and the current date.", result.errorMessage);
        }

        [TestMethod]
        public void CreateUser_InvalidPassword_ReturnsErrorResult()
        {
            // Arrange
            var dataAccessObject = new DataAccessObject(TestConnectionString);
        
            // Act
            var result = dataAccessObject.createUser("John", "Doe", "johndoe", "john.doe@email.com", new DateTime(1990, 1, 1), "weakpassword");

            // Assert
            Assert.IsTrue(result.hasError);
            Assert.IsNotNull(result.errorMessage);
            Assert.AreEqual("Invalid password format. Must be more than 8 characters and contain uppercase and lowercase letters as well as a single digit.", result.errorMessage);
        }

        [TestMethod]
        public void SaveLogAsync_InvalidConnectionString_ThrowsException()
        {
            // Arrange
            var invalidConnectionString = "invalid_connection_string";
            var dataAccessObject = new DataAccessObject(invalidConnectionString);
            var logMessage = "Test log message";
            var logLevel = LogLevel.Info;
            var logCategory = LogCategory.Business;

            // Act & Assert
            Assert.ThrowsAsync<SqlException>(() => dataAccessObject.SaveLogAsync(logMessage, logLevel, logCategory));
        }



        [TestMethod]
        public void CreateUser_DuplicateUsername_ReturnsErrorResult()
        {
            // Arrange
            var dataAccessObject = new DataAccessObject(TestConnectionString);
            dataAccessObject.createUser("John", "Doe", "existinguser", "john.doe@email.com", new DateTime(1990, 1, 1), "SecureP@ssword");
        
            // Act
            var result = dataAccessObject.createUser("Jane", "Doe", "existinguser", "jane.doe@email.com", new DateTime(1992, 2, 2), "AnotherSecurePassword");

            // Assert
            Assert.IsTrue(result.hasError);
            Assert.IsNotNull(result.errorMessage);
            Assert.AreEqual("Username 'existinguser' is already taken.", result.errorMessage);
        }

        [TestMethod]
        public async Task SaveLogAsync_EmptyMessage_ThrowsException()
        {
            // Arrange
            var dataAccessObject = new DataAccessObject(TestConnectionString);
            var logMessage = string.Empty;
            var logLevel = LogLevel.Warning;
            var logCategory = LogCategory.Server;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => dataAccessObject.SaveLogAsync(logMessage, logLevel, logCategory));
        }

        [TestMethod]
        public async Task SaveLogAsync_NullMessage_ThrowsException()
        {
            // Arrange
            var dataAccessObject = new DataAccessObject(TestConnectionString);
            string logMessage = null;
            var logLevel = LogLevel.Error;
            var logCategory = LogCategory.Data;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => dataAccessObject.SaveLogAsync(logMessage, logLevel, logCategory));
        }
        */
    }
}