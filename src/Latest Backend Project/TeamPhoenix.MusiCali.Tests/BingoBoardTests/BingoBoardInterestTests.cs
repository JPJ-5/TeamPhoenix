using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;

namespace TeamPhoenix.MusiCali.Tests.BingoBoardTests
{
    [TestClass]
    public class BingoBoardInterestViewTest
    {
        private readonly IConfiguration configuration;

        public BingoBoardInterestViewTest()
        {
            // Build configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();
        }


        [TestMethod]
        public void isUserAlreadyInterestedTest()
        {
            BingoBoardService bingoBoard = new BingoBoardService(configuration);
            bool isInserted = bingoBoard.IsUserInterested("bingoboardtests", 38);
            //Console.WriteLine(gigs[0].Username);
            Assert.IsTrue(isInserted);
        }
    }
}
