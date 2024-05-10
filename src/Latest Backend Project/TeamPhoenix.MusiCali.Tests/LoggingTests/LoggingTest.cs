using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using Microsoft.Extensions.Configuration;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class LoggingLibraryTest
    {
        private readonly IConfiguration configuration;
        private LoggerService loggerService;

        public LoggingLibraryTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            loggerService = new LoggerService(configuration);
        }

        [TestMethod]
        public void CreateLog_And_Returns_Success()
        {
            string hash = "testHash";
            string level = "test level";
            string category = "test category";
            string context = "test context";
            Result logResult = loggerService.CreateLog(hash, level, category, context);

            Assert.IsFalse(logResult.HasError);

        }

    }
}
