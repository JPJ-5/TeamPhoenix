using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class SqlDaoTest
    {

        private readonly IConfiguration configuration;
        private UserCreationService userCreationService;
        private UserDeletionService userDeletionService;
        private SqlDAO sqlDAO;

        public SqlDaoTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            userCreationService = new UserCreationService(configuration);
            userDeletionService = new UserDeletionService(configuration);
            sqlDAO = new SqlDAO(configuration);
        }


        [TestMethod]
        public void DataAccess_ShouldReturnSuccessfulConnection()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";

            // Act
            var result = sqlDAO.ExecuteSql(sql);

            //Assert
            Assert.IsTrue(result.Success);
            userDeletionService.DeleteUser("ExampleUser");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnSuccessfulRecordCreation()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";

            // Act
            var result = sqlDAO.ExecuteSql(sql);

            //Assert
            Assert.IsTrue(result.Success);
            userDeletionService.DeleteUser("ExampleUser");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForNullSqlCommand()
        {
            // Arrange
            string sql = "";

            // Act
            var result = sqlDAO.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForInvalidNullValueInSqlCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES (NULL, 'ExampleSalt', 'ExampleHash', 'Example@Email')";

            // Act
            var result = sqlDAO.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnTrueForReadingValue()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to read
            sql = "SELECT * FROM UserAccount WHERE Username = 'ExampleUser'";

            // Act
            var result = sqlDAO.ReadSql(sql);

            //Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.value);
            userDeletionService.DeleteUser("ExampleUser");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForReadingNonExistentValue()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to read
            sql = "SELECT * FROM UserAccount WHERE Username = 'ExampleFakeUser'";

            // Act
            var result = sqlDAO.ReadSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.value);
            userDeletionService.DeleteUser("ExampleUser");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForReadingNullSqlCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to read
            sql = "";

            // Act
            var result = sqlDAO.ReadSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.value);
            userDeletionService.DeleteUser("ExampleUser");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForReadingInvalidSqlCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to read
            sql = "SELETC * FROM UserAccount WHERE Username = 'ExampleUser'"; // Typo to show incorrect command.

            // Act
            var result = sqlDAO.ReadSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.value);
            userDeletionService.DeleteUser("ExampleUser");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnTrueForSuccessfulUpdate()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to update
            sql = "UPDATE UserAccount SET Email = 'Example2@Email' WHERE Username = 'ExampleUser'";

            // Act
            var result = sqlDAO.ExecuteSql(sql);

            //Assert
            Assert.IsTrue(result.Success);
            userDeletionService.DeleteUser("ExampleUser");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForUpdatingNonexistentRow()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to update
            sql = "UPDATE UserAccount SET Email = 'Example2@Email' WHERE Username = 'ExampleUser2'";

            // Act
            var result = sqlDAO.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
            userDeletionService.DeleteUser("ExampleUser");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForUpdatingNullCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to update
            sql = "";

            // Act
            var result = sqlDAO.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
            userDeletionService.DeleteUser("ExampleUser");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForUpdatingInvalidSqlCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to update
            sql = "UPDTAE UserAccount SET Email = 'Example2@Email' WHERE Username = 'ExampleUser'"; // typo to show invalid update command.

            // Act
            var result = sqlDAO.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
            userDeletionService.DeleteUser("ExampleUser");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnTrueForSuccessfulDelete()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to delete
            sql = "DELETE FROM UserAccount WHERE Username = 'ExampleUser'";

            // Act
            var result = sqlDAO.ExecuteSql(sql);

            //Assert
            Assert.IsTrue(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForDeleteNonexistentRow()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to delete
            sql = "DELETE FROM UserAccount WHERE Username = 'ExampleUser2'";

            // Act
            var result = sqlDAO.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
            userDeletionService.DeleteUser("ExampleUser");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForDeleteEmptySql()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to delete
            sql = "";

            // Act
            var result = sqlDAO.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
            userDeletionService.DeleteUser("ExampleUser");
        }

        [TestMethod]
        public void DataAccess_ShouldReturnFalseForDeleteInvalidSqlCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            sqlDAO.ExecuteSql(sql); // Add this value to table to delete
            sql = "DELTEE FROM UserAccount WHERE Username = 'ExampleUser'"; // Typo to show invalid sql command.

            // Act
            var result = sqlDAO.ExecuteSql(sql);
            userDeletionService.DeleteUser("ExampleUser");

            //Assert
            Assert.IsFalse(result.Success);
        }
    }
}
