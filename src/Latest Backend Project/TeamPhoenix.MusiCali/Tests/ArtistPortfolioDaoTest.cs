using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class ArtistPortfolioDaoTest
    {
        [TestMethod]
        public async Task SaveFilePath_ShouldUpdateFilePath_WhenSlotIsZero()
        {
            // Arrange
            var username = "juliereyes";
            var slot = 0;
            var filePath = "test_file_path.jpg";
            var genre = "Test Genre";
            var desc = "Test Description";

            // Act
            var result = ArtistPortfolioDao.SaveFilePath(username, slot, filePath, genre, desc);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);

            // Clean up: Delete the file path
            ArtistPortfolioDao.DeleteFilePath(username, slot);
        }

        [TestMethod]
        public async Task SaveFilePath_ShouldUpdateFilePath_WhenSlotIsNotZero()
        {
            // Arrange
            var username = "juliereyes";
            var slot = 1;
            var filePath = "test_file_path.mp3";
            var genre = "Test Genre";
            var desc = "Test Description";

            // Act
            var result = ArtistPortfolioDao.SaveFilePath(username, slot, filePath, genre, desc);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);

            // Clean up: Delete the file path
            ArtistPortfolioDao.DeleteFilePath(username, slot);
        }

        [TestMethod]
        public void DeleteFilePath_ShouldSetFilePathToNull_WhenSlotIsZero()
        {
            // Arrange
            var username = "juliereyes";
            var slot = 0;

            // Act
            var result = ArtistPortfolioDao.DeleteFilePath(username, slot);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public void DeleteFilePath_ShouldSetFilePathToNull_WhenSlotIsNotZero()
        {
            // Arrange
            var username = "juliereyes";
            var slot = 1;

            // Act
            var result = ArtistPortfolioDao.DeleteFilePath(username, slot);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public void GetFilePath_ShouldReturnFilePath_WhenSlotIsZero()
        {
            // Arrange
            var username = "juliereyes";
            var slot = 0;
            var expectedFilePath = "test_file_path.jpg";
            ArtistPortfolioDao.SaveFilePath(username, slot, expectedFilePath, "", "");

            // Act
            var actualFilePath = ArtistPortfolioDao.GetFilePath(username, slot);

            // Assert
            Assert.AreEqual(expectedFilePath, actualFilePath);

            // Clean up: Delete the file path
            ArtistPortfolioDao.DeleteFilePath(username, slot);
        }

        [TestMethod]
        public void GetFilePath_ShouldReturnFilePath_WhenSlotIsNotZero()
        {
            // Arrange
            var username = "juliereyes";
            var slot = 1;
            var expectedFilePath = "test_file_path.mp3";
            ArtistPortfolioDao.SaveFilePath(username, slot, expectedFilePath, "", "");

            // Act
            var actualFilePath = ArtistPortfolioDao.GetFilePath(username, slot);

            // Assert
            Assert.AreEqual(expectedFilePath, actualFilePath);

            // Clean up: Delete the file path
            ArtistPortfolioDao.DeleteFilePath(username, slot);
        }

        [TestMethod]
        public void GetUsername_ShouldReturnUsername_WhenUsernameExists()
        {
            // Arrange
            var username = "juliereyes";
            ArtistPortfolioDao.SaveFilePath(username, 0, "", "", "");

            // Act
            var actualUsername = ArtistPortfolioDao.GetUsername(username);

            // Assert
            Assert.AreEqual(username, actualUsername);

            // Clean up: Delete the file path
            ArtistPortfolioDao.DeleteFilePath(username, 0);
        }

        [TestMethod]
        public void GetProfileInfo_ShouldReturnProfileInfo_WhenUsernameExists()
        {
            // Arrange
            var username = "juliereyes";
            var expectedOccupation = "Artist";
            var expectedBio = "Into shoegaze, alt, indie music and jungle, phonk, and house for edm";
            var expectedLocation = "Los Angeles";
            ArtistPortfolioDao.SaveFilePath(username, 0, "", "", "");

            // Act
            var profileInfo = ArtistPortfolioDao.GetProfileInfo(username);
            var actualOccupation = profileInfo[0];
            var actualBio = profileInfo[1];
            var actualLocation = profileInfo[2];

            // Assert
            Assert.AreEqual(expectedOccupation, actualOccupation);
            Assert.AreEqual(expectedBio, actualBio);
            Assert.AreEqual(expectedLocation, actualLocation);

            // Clean up: Delete the file path
            ArtistPortfolioDao.DeleteFilePath(username, 0);
        }

        [TestMethod]
        public void GetAllFileInfo_ShouldReturnFileInfo_WhenUsernameExists()
        {
            // Arrange
            var username = "juliereyes";
            var expectedFilePath = "test_file_path.mp3";
            ArtistPortfolioDao.SaveFilePath(username, 1, expectedFilePath, "", "");

            // Act
            var fileData = ArtistPortfolioDao.GetAllFileInfo(username);
            var actualFilePath = fileData[0][1];

            // Assert
            Assert.AreEqual(expectedFilePath, actualFilePath);

            // Clean up: Delete the file path
            ArtistPortfolioDao.DeleteFilePath(username, 1);
        }
    }
}
