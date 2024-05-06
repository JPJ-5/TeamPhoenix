using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer;
using System.Configuration;
using TeamPhoenix.MusiCali.Tests.Models;

namespace Teamphoenix.Musicali.Tests
{
    [TestClass]
    public class ArtistPortfolioDaoTest
    {
        private readonly IConfiguration configuration;
        private ArtistPortfolioDao artistPortfolioDao;

        public ArtistPortfolioDaoTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();
            artistPortfolioDao = new ArtistPortfolioDao(configuration);
        }

        [TestMethod]
        public async Task savefilepath_shouldupdatefilepath_whenslotiszero()
        {
            // arrange
            var username = "kihambo.wav";
            var slot = 3;
            var filepath = "test_file_path.jpg";
            var genre = "test genre.....";
            var desc = "test description";

            // act
            var result = artistPortfolioDao.SaveFilePath(username, slot, filepath, genre, desc);
            await Task.FromResult(result);

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);

            // clean up: delete the file path
            artistPortfolioDao.DeleteFilePath(username, slot);
        }

        [TestMethod]
        public async Task savefilepath_shouldupdatefilepath_whenslotisnotzero()
        {
            // arrange
            var username = "kihambo.wav";
            var slot = 4;
            var filepath = "test_file_path.mp3";
            var genre = "test genre....";
            var desc = "test description";

            // act
            var result = artistPortfolioDao.SaveFilePath(username, slot, filepath, genre, desc);
            await Task.FromResult(result);

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);

            // clean up: delete the file path
        }

        [TestMethod]
        public async Task deletefilepath_shouldsetfilepathtonull_whenslotiszero()
        {
            // arrange
            var username = "kihambo.wav";
            var slot = 4;

            // act
            var result = artistPortfolioDao.DeleteFilePath(username, slot);
            await Task.FromResult(result);

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task deletefilepath_shouldsetfilepathtonull_whenslotisnotzero()
        {
            // arrange
            var username = "kihambo.wav";
            var slot = 2;

            // act
            var result = artistPortfolioDao.DeleteFilePath(username, slot);
            await Task.FromResult(result);

            // assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public void getfilepath_shouldreturnfilepath_whenslotiszero()
        {
            // arrange
            var username = "kihambo.wav";
            var slot = 0;
            var expectedfilepath = "test_file_path.jpg";
            var result = artistPortfolioDao.SaveFilePath(username, slot, expectedfilepath, "", "");

            // act
            var actualfilepath = artistPortfolioDao.GetFilePath(username, slot);

            var res = artistPortfolioDao.DeleteFilePath(username, 0);

            // assert
            Assert.AreEqual(expectedfilepath, actualfilepath);

        }

        [TestMethod]
        public void getfilepath_shouldreturnfilepath_whenslotisnotzero()
        {
            // arrange
            var username = "kihambo.wav";
            var slot = 1;
            var expectedfilepath = "test_file_path.mp3";
            artistPortfolioDao.SaveFilePath(username, slot, expectedfilepath, "", "");

            // act
            var actualfilepath = artistPortfolioDao.GetFilePath(username, slot);

            // assert
            Assert.AreEqual(expectedfilepath, actualfilepath);

            // clean up: delete the file path
            artistPortfolioDao.DeleteFilePath(username, slot);
        }


        [TestMethod]
        public void getprofileinfo_shouldreturnprofileinfo_whenusernameexists()
        {
            // arrange
            var username = "juliereyes";
            var expectedoccupation = "Instrumentalist";
            var expectedbio = "Lead of MusiCali into edm and most rock genres(alt, punk, indie, etc.)";
            var expectedlocation = "Los Angeles";

            // act
            var portfolio = artistPortfolioDao.GetPortfolio(username);
            var profileinfo = portfolio[3];
            var actualoccupation = profileinfo[0];
            var actualbio = profileinfo[1];
            var actuallocation = profileinfo[2];

            // assert
            Assert.AreEqual(expectedoccupation, actualoccupation);
            Assert.AreEqual(expectedbio, actualbio);
            Assert.AreEqual(expectedlocation, actuallocation);

        }

    }
}
