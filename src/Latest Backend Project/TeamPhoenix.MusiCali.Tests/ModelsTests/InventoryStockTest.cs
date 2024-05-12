using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class InventoryStockTest
    {
        [TestMethod]
        public void InventoryStockTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var stockModel = new InventoryStockModel();

            // Assert
            Assert.IsNull(stockModel.Name);
            Assert.IsNull(stockModel.SKU);
            Assert.AreEqual(0, stockModel.StockAvailable);
            Assert.IsNull(stockModel.Price);
            Assert.IsNull(stockModel.DateCreated);
        }
        [TestMethod]
        public void InventoryStockTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {

            // Arrange & Act
            DateTime testDate = DateTime.Now;
            var testInventoryStockModel = new InventoryStockModel
            {
                Name = "Product",
                SKU = "ABC123",
                StockAvailable = 10,
                Price = 15.99m,
                DateCreated = testDate
            };

            // Assert
            Assert.AreEqual("Product", testInventoryStockModel.Name);
            Assert.AreEqual("ABC123", testInventoryStockModel.SKU);
            Assert.AreEqual(10, testInventoryStockModel.StockAvailable);
            Assert.AreEqual(15.99m, testInventoryStockModel.Price);
            Assert.AreEqual(testDate, testInventoryStockModel.DateCreated);
        }
    }
}
