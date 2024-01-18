using System;
using uc = TeamPhoenix.MusiCali.Services.UserCreation;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{
    [TestClass]
    public class SqlDAOTest
    {
        private SqlDAO dao;
        [TestInitialize]
        public void TestInitialize()
        {
            dao = new SqlDAO("julie", "j1234");
        }
        [TestMethod]
        public void DataAccess_ShouldReturnSuccessfulConnection()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsTrue(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnSuccessfulRecordCreation()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsTrue(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForNullSqlCommand()
        {
            // Arrange
            string sql = "";

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForInvalidNullValueInSqlCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES (NULL, 'ExampleSalt', 'ExampleHash', 'Example@Email')";

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnTrueForReadingValue()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to read
            sql = "SELECT * FROM UserAccount WHERE Username = 'ExampleUser'";

            // Act
            var result = dao.ReadSql(sql);

            //Assert
            Assert.IsTrue(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForReadingNonExistentValue()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to read
            sql = "SELECT * FROM UserAccount WHERE Username = 'ExampleFakeUser'";

            // Act
            var result = dao.ReadSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForReadingNullSqlCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to read
            sql = "";

            // Act
            var result = dao.ReadSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForReadingInvalidSqlCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to read
            sql = "SELETC * FROM UserAccount WHERE Username = 'ExampleUser'"; // Typo to show incorrect command.

            // Act
            var result = dao.ReadSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnTrueForSuccessfulUpdate()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to update
            sql = "UPDATE UserAccount SET Email = 'Example2@Email' WHERE Username = 'ExampleUser'";

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsTrue(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForUpdatingNonexistentRow()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to update
            sql = "UPDATE UserAccount SET Email = 'Example2@Email' WHERE Username = 'ExampleUser2'";

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForUpdatingNullCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to update
            sql = "";

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        public void DataAccess_ShouldReturnFalseForUpdatingInvalidSqlCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to update
            sql = "UPDTAE UserAccount SET Email = 'Example2@Email' WHERE Username = 'ExampleUser'"; // typo to show invalid update command.

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnTrueForSuccessfulDelete()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to delete
            sql = "DELETE FROM UserAccount WHERE Username = 'ExampleUser'";

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsTrue(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForDeleteNonexistentRow()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to delete
            sql = "DELETE FROM UserAccount WHERE Username = 'ExampleUser2'";

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        [TestMethod]
        public void DataAccess_ShouldReturnFalseForDeleteEmptySql()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to delete
            sql = "";

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        public void DataAccess_ShouldReturnFalseForDeleteInvalidSqlCommand()
        {
            // Arrange
            string sql = "INSERT INTO UserAccount (Username, Salt, UserHash, Email) VALUES ('ExampleUser', 'ExampleSalt', 'ExampleHash', 'Example@Email')";
            dao.ExecuteSql(sql); // Add this value to table to delete
            sql = "DELTEE FROM UserAccount WHERE Username = 'ExampleUser'"; // Typo to show invalid sql command.

            // Act
            var result = dao.ExecuteSql(sql);

            //Assert
            Assert.IsFalse(result.Success);
        }
        [TestCleanup]
        public void Cleanup()
        {
            Tester.DeleteAllRows("UserAccount");
        }
    }
}
