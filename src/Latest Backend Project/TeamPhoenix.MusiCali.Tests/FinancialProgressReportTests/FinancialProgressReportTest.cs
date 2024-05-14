using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class FinancialProgressReportTests
    {
        private readonly IConfiguration configuration;
        private FinancialProgressReportDAO dao;

        public FinancialProgressReportTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            dao = new FinancialProgressReportDAO(configuration);
        }

        [TestMethod]
        public void FetchYearlyReport_ValidUserHash_ReturnsYearlyFinancialData()
        {
            // Arrange
            var userHash = "eb4e92b99829441156353cb27f7897de0e0258bd15e8e583398d2b697bfb4788";

            // Act
            var result = dao.FetchYearlyReport(userHash);

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.IsTrue(result.Count > 0, "Result should contain at least one record.");
        }

        [TestMethod]
        public void FetchYearlyReport_InvalidUserHash_ReturnsEmptyResult()
        {
            // Arrange
            var userHash = "eb4e92b99829441156353cb27fsaf7de0esad8bd15e8e583398d2b697bfb4788";

            // Act
            var result = dao.FetchYearlyReport(userHash);

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.AreEqual(0, result.Count, "Result should be empty.");
        }

        [TestMethod]
        public void FetchQuarterlyReport_ValidUserHash_ReturnsQuarterlyFinancialData()
        {
            // Arrange
            var userHash = "eb4e92b99829441156353cb27f7897de0e0258bd15e8e583398d2b697bfb4788";

            // Act
            var result = dao.FetchQuarterlyReport(userHash);

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.IsTrue(result.Count > 0, "Result should contain at least one record.");
        }

        [TestMethod]
        public void FetchQuarterlyReport_InvalidUserHash_ReturnsEmptyResult()
        {
            // Arrange
            var userHash = "eb4gh2bty82944115ug53cb27f7897de0e0258bd15e8e583398d2b697bfb4788";

            // Act
            var result = dao.FetchQuarterlyReport(userHash);

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.AreEqual(0, result.Count, "Result should be empty.");
        }

        
        [TestMethod]
        public void FetchMonthlyReport_ValidUserHash_ReturnsMonthlyFinancialData()
        {
            // Arrange
            var userHash = "eb4e92b99829441156353cb27f7897de0e0258bd15e8e583398d2b697bfb4788";

            // Act
            var result = dao.FetchMonthlyReport(userHash);

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.IsTrue(result.Count > 0, "Result should contain at least one record.");
        }

        [TestMethod]
        public void FetchMonthlyReport_InvalidUserHash_ReturnsEmptyResult()
        {
            // Arrange
            var userHash = "eb4e92b99359441776353cb27f7897de0e0258bd15e8e583398d2b697bfb4788";

            // Act
            var result = dao.FetchMonthlyReport(userHash);

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.AreEqual(0, result.Count, "Result should be empty.");
        }
    }
}