using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class CollabSearchDaoTest
    {
        [TestMethod]
        public async Task SearchUsers_ShouldReturnMatchingUsers_WhenQueryIsProvided()
        {
            // Arrange
            var query = "UserSearch";
            var expectedUsers = new List<string> { "UserSearch1", "UserSearch2", "UserSearch3" };

            // Act
            var matchingUsers = CollabSearchDao.SearchUsers(query);

            // Assert
            CollectionAssert.AreEqual(expectedUsers, matchingUsers);
        }

        [TestMethod]
        public async Task SearchUsers_ShouldReturnEmptyList_WhenNoMatchingUsersFound()
        {
            // Arrange
            var query = "NonExistentUser";

            // Act
            var matchingUsers = CollabSearchDao.SearchUsers(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }

        [TestMethod]
        public void SearchUsers_ShouldReturnEmptyList_WhenQueryIsNull()
        {
            // Arrange
            string query = null;

            // Act
            var matchingUsers = CollabSearchDao.SearchUsers(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }

        [TestMethod]
        public void SearchUsers_ShouldReturnEmptyList_WhenQueryIsEmpty()
        {
            // Arrange
            var query = "";

            // Act
            var matchingUsers = CollabSearchDao.SearchUsers(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }

        [TestMethod]
        public void SearchUsers_ShouldReturnMatchingUsersWithVisibilityTrue_WhenQueryIsProvided()
        {
            // Arrange
            var query = "UserSearch";
            var expectedUsers = new List<string> { "UserSearch1", "UserSearch2", "UserSearch3" };

            // Act
            var matchingUsers = CollabSearchDao.SearchUsersWithVisibility(query);

            // Assert
            CollectionAssert.AreEqual(expectedUsers, matchingUsers);
        }

        [TestMethod]
        public void SearchUsersWithVisibility_ShouldReturnEmptyList_WhenNoMatchingUsersFound()
        {
            // Arrange
            var query = "NonExistentUser";

            // Act
            var matchingUsers = CollabSearchDao.SearchUsersWithVisibility(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }

        [TestMethod]
        public void SearchUsersWithVisibility_ShouldReturnEmptyList_WhenQueryIsNull()
        {
            // Arrange
            string query = null;

            // Act
            var matchingUsers = CollabSearchDao.SearchUsersWithVisibility(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }

        [TestMethod]
        public void SearchUsersWithVisibility_ShouldReturnEmptyList_WhenQueryIsEmpty()
        {
            // Arrange
            var query = "";

            // Act
            var matchingUsers = CollabSearchDao.SearchUsersWithVisibility(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }
    }
}