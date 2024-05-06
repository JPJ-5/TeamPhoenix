using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.OutputCaching;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class ArtistPortfolioTest
    {
        private readonly IConfiguration configuration;
        private ArtistPortfolio artistPortfolio;

        public ArtistPortfolioTest() 
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();
            artistPortfolio = new ArtistPortfolio(configuration);
        }

        [TestMethod]
        public async Task UploadFile_ShouldReturnSuccess_WhenFileIsValid()
        {
            // Arrange
            var username = "kihambo.wav";

            // Load the test file 'tick.mp' from the test files folder
            var filePath = @"..\..\..\ArtistPortfolioTests\tick.mp3";
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var formFile = new FormFile(new MemoryStream(fileBytes), 2, fileBytes.Length, "", "tick.wav");

            // Act
            var result = await artistPortfolio.UploadFile(username, 2, formFile, "wav", "tick sound but in wav");

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);
        }


        [TestMethod]
        public void LoadProfileValid()
        {
            // Arrange
            var username = "juliereyes";

            ArtistProfileViewModel profile = artistPortfolio.LoadArtistProfile(username);
            var name = "smoothie-liquid-dnb--jungle_TK15603481.mp3.mp3";
            var occ = "Instrumentalist";

            // Assert
            Assert.AreEqual(profile.File1Name, name);
            Assert.AreEqual(profile.Occupation, occ);
        }


        [TestMethod]
        public void DeleteFile_and_filepath()
        {
            // Arrange
            var username = "kihambo.wav";

            int slot = 2;
            var result = artistPortfolio.DeleteFile(username, slot);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);
        }

    }
}
