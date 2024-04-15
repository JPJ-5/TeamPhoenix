using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class CollabSearchDaoTest
    {
        [TestMethod]
        public void SearchUsers_ShouldReturnMatchingUsers_WhenQueryIsProvided()
        {
            // Arrange
            var query = "julierey";
            List<string> expectedUsers = new List<string> { "juliereyes" };

            // Act
            var matchingUsers = CollabFeatureDAL.SearchUsers(query);


            // Assert
            Console.WriteLine("match: " + matchingUsers.Count);
            CollectionAssert.AreEqual(expectedUsers, matchingUsers);
        }

        [TestMethod]
        public void SearchUsers_ShouldReturnEmptyList_WhenNoMatchingUsersFound()
        {
            // Arrange
            var query = "NonExistentUser";

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
    }
}
