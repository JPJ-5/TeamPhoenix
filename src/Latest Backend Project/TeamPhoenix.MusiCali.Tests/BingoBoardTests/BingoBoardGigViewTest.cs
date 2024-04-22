using Microsoft.Extensions.Configuration;
using System.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;


namespace TeamPhoenix.MusiCali.Tests.BingoBoardTests
{
    [TestClass]
    public class BingoBoardGigViewTest
    {
        private readonly IConfiguration configuration;

        public BingoBoardGigViewTest()
        {
            // Build configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();
        }
        [TestMethod]
        public void GigLoadTest()
        {
            BingoBoardDAO bingoBoardDAO = new BingoBoardDAO(configuration);
            GigSet? gigs = bingoBoardDAO.ViewGigSummary(20, "bingoboardtests", 0);
            //Console.WriteLine(gigs[0].Username);
            Assert.IsNotNull(gigs);
        }

        [TestMethod]
        public void GigNumTest()
        {
            BingoBoardDAO bingoBoardDAO = new BingoBoardDAO(configuration);
            int numOfGigs = bingoBoardDAO.ReturnNumOfGigs();
            Assert.IsNotNull(numOfGigs);
        }
    }
}
