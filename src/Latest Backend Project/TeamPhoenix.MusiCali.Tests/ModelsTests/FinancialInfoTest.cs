using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class FinancialInfoTest
    {
        [TestMethod]
        public void FinancialInfoTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var financialInfo = new FinancialInfoModel();

            // Assert
            Assert.IsNull(financialInfo.financialMonth);
            Assert.IsNull(financialInfo.financialQuater);
            Assert.AreEqual(0, financialInfo.financialYear);
            Assert.AreEqual(0m, financialInfo.financialProfit);
            Assert.AreEqual(0m, financialInfo.financialRevenue);
            Assert.AreEqual(0, financialInfo.sales);
        }
        [TestMethod]
        public void FinancialInfoTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {
            // Arrange
            int expectedFinancialMonth = 3;
            int expectedFinancialQuater = 2;
            int expectedFinancialYear = 2024;
            decimal expectedFinancialProfit = 10000.50m;
            decimal expectedFinancialRevenue = 50000.75m;
            int expectedSales = 100;

            // Act
            var financialInfo = new FinancialInfoModel
            {
                financialMonth = expectedFinancialMonth,
                financialQuater = expectedFinancialQuater,
                financialYear = expectedFinancialYear,
                financialProfit = expectedFinancialProfit,
                financialRevenue = expectedFinancialRevenue,
                sales = expectedSales
            };

            // Assert
            Assert.AreEqual(expectedFinancialMonth, financialInfo.financialMonth);
            Assert.AreEqual(expectedFinancialQuater, financialInfo.financialQuater);
            Assert.AreEqual(expectedFinancialYear, financialInfo.financialYear);
            Assert.AreEqual(expectedFinancialProfit, financialInfo.financialProfit);
            Assert.AreEqual(expectedFinancialRevenue, financialInfo.financialRevenue);
            Assert.AreEqual(expectedSales, financialInfo.sales);
        }
    }
}
