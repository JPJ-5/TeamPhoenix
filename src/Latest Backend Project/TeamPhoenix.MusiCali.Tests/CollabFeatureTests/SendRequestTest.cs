using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
     [TestClass]
     public class SendRequestTest
     {
        [TestMethod]
        public async Task SearchUsers_ShouldReturnMatchingUsers_WhenQueryIsProvided()
        {
            // Arrange
            var query = "juliereyes";
            var expectedUsers = new List<string> { "juliereyes" };

            // Act
            var matchingUsers = CollabFeatureDAL.SearchUsers(query);

            // Assert
            CollectionAssert.AreEqual(expectedUsers, matchingUsers);
        }

        [TestMethod]
        public async Task SearchUsers_ShouldReturnEmptyList_WhenNoMatchingUsersFound()
        {
            // Arrange
            var query = "NonExistingUser";

            // Act
            var matchingUsers = CollabFeatureDAL.SearchUsers(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }

        [TestMethod]
        public void SearchUsers_ShouldReturnEmptyList_WhenQueryIsNull()
        {
            // Arrange
            string query = null;

            // Act
            var matchingUsers = CollabFeatureDAL.SearchUsers(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }

        [TestMethod]
        public void SearchUsers_ShouldReturnEmptyList_WhenQueryIsEmpty()
        {
            // Arrange
            var query = "";

            // Act
            var matchingUsers = CollabFeatureDAL.SearchUsers(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }

        [TestMethod]
        public void SearchUsersWithVisibility_ShouldReturnEmptyList_WhenNoMatchingUsersFound()
        {
            // Arrange
            var query = "NonExistentUser";

            // Act
            var matchingUsers = CollabFeatureDAL.SearchUsers(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }

        [TestMethod]
        public void SearchUsersWithVisibility_ShouldReturnEmptyList_WhenQueryIsNull()
        {
            // Arrange
            string query = null;

            // Act
            var matchingUsers = CollabFeatureDAL.SearchUsers(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }

        [TestMethod]
        public void SearchUsersWithVisibility_ShouldReturnEmptyList_WhenQueryIsEmpty()
        {
            // Arrange
            var query = " ";

            // Act
            var matchingUsers = CollabFeatureDAL.SearchUsers(query);

            // Assert
            Assert.AreEqual(0, matchingUsers.Count);
        }
    }
}
