using System.Security.Claims;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class ItemCreationTest
    {
        [TestMethod]
        public void ItemCreationTest_ShouldReturnTrueForSuccessfulDefaultModel()
        {
            // Arrange & Act
            var itemModel = new ItemCreationModel();

            // Assert
            Assert.IsNull(itemModel.Name);
            Assert.IsNull(itemModel.CreatorHash);
            Assert.IsNull(itemModel.Sku);
            Assert.AreEqual(0, itemModel.Price);
            Assert.IsNull(itemModel.Description);
            Assert.AreEqual(0, itemModel.StockAvailable);
            Assert.AreEqual(0, itemModel.ProductionCost);
            Assert.IsFalse(itemModel.OfferablePrice);
            Assert.IsNull(itemModel.SellerContact);
            Assert.IsNull(itemModel.ImageUrls);
            Assert.IsNull(itemModel.VideoUrls);
            Assert.AreEqual(DateTime.Now.Date, itemModel.DateCreated.Date);
        }
        [TestMethod]
        public void ItemCreationTest_ShouldReturnTrueForSuccessfulPropertyChange()
        {

            // Arrange
            string expectedName = "Test Item";
            string expectedCreatorHash = "12345";
            string expectedSku = "SKU123";
            decimal expectedPrice = 99.99m;
            string expectedDescription = "Test Description";
            int expectedStockAvailable = 10;
            decimal expectedProductionCost = 50.00m;
            bool expectedOfferablePrice = true;
            string expectedSellerContact = "test@example.com";
            var expectedImageUrls = new List<string> { "url1", "url2" };
            var expectedVideoUrls = new List<string> { "video1", "video2" };
            DateTime expectedDateCreated = DateTime.Now;

            // Act
            var itemModel = new ItemCreationModel
            {
                Name = expectedName,
                CreatorHash = expectedCreatorHash,
                Sku = expectedSku,
                Price = expectedPrice,
                Description = expectedDescription,
                StockAvailable = expectedStockAvailable,
                ProductionCost = expectedProductionCost,
                OfferablePrice = expectedOfferablePrice,
                SellerContact = expectedSellerContact,
                ImageUrls = expectedImageUrls,
                VideoUrls = expectedVideoUrls,
                DateCreated = expectedDateCreated
            };

            // Assert
            Assert.AreEqual(expectedName, itemModel.Name);
            Assert.AreEqual(expectedCreatorHash, itemModel.CreatorHash);
            Assert.AreEqual(expectedSku, itemModel.Sku);
            Assert.AreEqual(expectedPrice, itemModel.Price);
            Assert.AreEqual(expectedDescription, itemModel.Description);
            Assert.AreEqual(expectedStockAvailable, itemModel.StockAvailable);
            Assert.AreEqual(expectedProductionCost, itemModel.ProductionCost);
            Assert.AreEqual(expectedOfferablePrice, itemModel.OfferablePrice);
            Assert.AreEqual(expectedSellerContact, itemModel.SellerContact);
            CollectionAssert.AreEqual(expectedImageUrls, itemModel.ImageUrls);
            CollectionAssert.AreEqual(expectedVideoUrls, itemModel.VideoUrls);
            Assert.AreEqual(expectedDateCreated, itemModel.DateCreated);
        }
    }
}
