using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class ArtistPortfolioDaoTest
    {
        [TestMethod]
        public async Task UploadFile_ShouldReturnSuccess_WhenFileIsValid()
        {
            // Arrange
            var username = "juliereyes";

            // Load the test file 'tick.mp' from the test files folder
            var filePath = @"..\..\..\Tests\tick2.mp3";
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var formFile = new FormFile(new MemoryStream(fileBytes), 1, fileBytes.Length, null, "tick.mp");

            // Act
            var result = await ArtistPortfolio.UploadFile(username,1, formFile, "", "");

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);
        }


        [TestMethod]
        public void getFileInfo()
        {
            // Arrange
            var username = "juliereyes";

            int slot = 1;
            List<List<string>> fileData = ArtistPortfolioDao.GetAllFileInfo(username);
            List<string> filePaths = fileData[0];

            // Assert
            Assert.IsNotNull(filePaths[1]);
        }


        [TestMethod]
        public void DeleteFile_and_filepath()
        {
            // Arrange
            var username = "juliereyes";

            int slot = 1;
            var result = ArtistPortfolio.DeleteFile(username, slot);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);
        }

    }
}
