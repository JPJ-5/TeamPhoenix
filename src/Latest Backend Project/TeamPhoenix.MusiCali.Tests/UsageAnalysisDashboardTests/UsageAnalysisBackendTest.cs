using System.Diagnostics; // for timing the length of Act.
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class UsageAnalysisBackendTest
    {
        private readonly IConfiguration configuration;
        private UsageAnalysisDashboardService usageAnalysisService;
        public UsageAnalysisBackendTest()
        {
            // Load configuration from appsettings.json in the test project
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            usageAnalysisService = new UsageAnalysisDashboardService(configuration);
        }
        [TestMethod]
        public void CreatePageLengthLog_ShouldReturnSuccessfulLog()
        {
            //Arrange
            string username = "ArtistCalendarTest";
            string context = "Artist Calendar Feature";
            int pageLength = 20000;
            var timer = new Stopwatch();

            //Act
            timer.Start();
            Result testResult = usageAnalysisService.CreatePageLengthLogService(username, context, pageLength);
            timer.Stop();

            //Assert
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);
            Assert.IsFalse(testResult.HasError);
        }
        [TestMethod]
        public void GetLoginWithinTimeframe_ShouldReturnSuccessfulResult()
        {
            //Arrange
            string username = "ArtistCalendarTest";
            int months = 3;
            var timer = new Stopwatch();

            //Act
            timer.Start();
            MonthYearCountResult testResult = usageAnalysisService.GetLoginWithinTimeframeService(username, months);
            timer.Stop();

            //Assert
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);
            Assert.IsTrue(testResult.Success);
            Assert.IsNotNull(testResult.Values);
        }
        [TestMethod]
        public void GetRegistrationWithinTimeframe_ShouldReturnSuccessfulResult()
        {
            //Arrange
            string username = "ArtistCalendarTest";
            int months = 3;
            var timer = new Stopwatch();

            timer.Start();
            MonthYearCountResult testResult = usageAnalysisService.GetRegistrationWithinTimeframeService(username, months);
            timer.Stop();
            //Assert
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);
            Assert.IsTrue(testResult.Success);
            Assert.IsNotNull(testResult.Values);
        }
        [TestMethod]
        public void GetGigsCreatedWithinTimeframe_ShouldReturnSuccessfulResult()
        {
            //Arrange
            string username = "ArtistCalendarTest";
            int months = 3;
            var timer = new Stopwatch();

            timer.Start();
            MonthYearCountResult testResult = usageAnalysisService.GetGigsCreatedWithinTimeframeService(username, months);
            timer.Stop();
            //Assert
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);
            Assert.IsTrue(testResult.Success);
            Assert.IsNotNull(testResult.Values);
        }
        [TestMethod]
        public void GetLongestPageViewWithinTimeframe_ShouldReturnSuccessfulResult()
        {
            //Arrange
            string username = "ArtistCalendarTest";
            int months = 3;
            var timer = new Stopwatch();

            timer.Start();
            PageViewLengthResult testResult = usageAnalysisService.GetLongestPageViewWithinTimeframeService(username, months);
            timer.Stop();
            //Assert
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);
            Assert.IsTrue(testResult.Success);
            Assert.IsNotNull(testResult.Values);
        }

        [TestMethod]
        public void GetItemsSoldWithinTimeframe_ShouldReturnSuccessfulResult()
        {
            //Arrange
            string username = "ArtistCalendarTest";
            int months = 3;
            var timer = new Stopwatch();

            timer.Start();
            ItemQuantityResult testResult = usageAnalysisService.GetItemsSoldWithinTimeframeService(username, months);
            timer.Stop();
            //Assert
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);
            Assert.IsTrue(testResult.Success);
            Assert.IsNotNull(testResult.Values);
        }
    }
}
