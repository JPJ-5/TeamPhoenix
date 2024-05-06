using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Tests
{
    [TestClass]
    public class ItemSortingTests
    {
        private readonly IConfiguration configuration;
        private DataAccessLayer dal;
        private ItemService service;

        public ItemSortingTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            dal = new DataAccessLayer(configuration);
            service = new ItemService(dal);
        }

        [TestMethod]
        public async Task FetchItems_ReturnsItemsWithinPriceRange()
        {
            var topPrice = 1000m;
            var bottomPrice = 10m;
            var items = await dal.FetchItems(topPrice, bottomPrice);

            Assert.IsTrue(items.All(item => item.Price <= topPrice && item.Price >= bottomPrice),
                "All items should be within the specified price range.");
        }

        [TestMethod]
        public async Task SortItemsByPriceRange_ReturnsCorrectlyOrderedItems()
        {
            var topPrice = 1000m;
            var bottomPrice = 100m;
            var items = await service.SortItemsByPriceRange(topPrice, bottomPrice);

            Assert.IsTrue(items.SequenceEqual(items.OrderBy(i => i.Price)),
                "Items should be ordered by price in ascending order.");
        }

        [TestMethod]
        public async Task Controller_SortItems_ReturnsBadRequestForInvalidInput()
        {
            var controller = new ItemController(service);
            var result = await controller.SortItems(50, 100) as BadRequestObjectResult;

            Assert.IsNotNull(result, "Expected BadRequest result for invalid price range.");
            Assert.AreEqual("Top price cannot be less than bottom price.", result.Value);
        }

        [TestMethod]
        public async Task Controller_SortItems_ReturnsNotFoundForNoItems()
        {
            var controller = new ItemController(service);
            var result = await controller.SortItems(999999, 900000) as NotFoundObjectResult;

            Assert.IsNotNull(result, "Expected NotFound result when no items are found within the price range.");
        }

        [TestMethod]
        public async Task Controller_SortItems_ReturnsItems()
        {
            var controller = new ItemController(service);
            var result = await controller.SortItems(1000, 100) as OkObjectResult;

            Assert.IsNotNull(result, "Expected Ok result with items.");
            Assert.IsInstanceOfType(result.Value, typeof(System.Collections.Generic.HashSet<Item>), "Expected the result to be a HashSet<Item>");
        }
    }
}
