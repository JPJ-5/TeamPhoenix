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
    public class ArtistPortfolioTest
    {
        private readonly IConfiguration config;

        public ArtistPortfolioTest(IConfiguration config)
        {
            this.config = config;
        }

        [TestMethod]
        public async Task UploadFile_ShouldReturnSuccess_WhenFileIsValid()
        {
            // Arrange
            var username = "kihambo.wav";

            // Load the test file 'tick.mp' from the test files folder
            var filePath = @"..\..\..\Tests\tick3.wav";
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var formFile = new FormFile(new MemoryStream(fileBytes), 2, fileBytes.Length, null, "tick.wav");

            // Act
            var result = await ArtistPortfolio.UploadFile(username, 2, formFile, "wav", "tick sound but in wav", config);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);
        }


        [TestMethod]
        public void getFileInfo()
        {
            // Arrange
            var username = "kihambo.wav";

            int slot = 1;
            List<List<string>> fileData = ArtistPortfolioDao.GetPortfolio(username);
            List<string> filePaths = fileData[0];

            // Assert
            Assert.IsNotNull(filePaths[1]);
        }


        [TestMethod]
        public void DeleteFile_and_filepath()
        {
            // Arrange
            var username = "kihambo.wav";

            int slot = 1;
            var result = ArtistPortfolio.DeleteFile(username, slot, config);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);
        }

    }
}
