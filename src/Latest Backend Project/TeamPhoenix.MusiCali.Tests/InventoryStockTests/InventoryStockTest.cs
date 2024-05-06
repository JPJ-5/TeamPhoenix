using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
//using static InventoryStockDAO;

namespace MyApp.Tests
{
    [TestClass]
    public class InventoryStockTests
    {
        private InventoryStockController? _controller;
        private IConfiguration? _configuration;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _configuration = builder.Build();

            var service = new InventoryStockService(_configuration);
            _controller = new InventoryStockController(_configuration);
        }

        [TestMethod]
        public async Task InventoryStockList_ReturnsQuickly()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var result = await _controller!.GetInventoryStock("creator1") as OkObjectResult;

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Assert.IsTrue(elapsedMs < 3000, "Inventory stock list should be returned within 3 seconds.");
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Expected a successful result.");
        }


        [TestMethod]
        public async Task InventoryStockList_ContainsNoDuplicates()
        {
            // Act
            var actionResult = await _controller!.GetInventoryStock("creator1");
            Console.WriteLine(actionResult);
            // Assert for IActionResult to be of type OkObjectResult and not null
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult), "Expected result to be an OkObjectResult.");
            var okResult = actionResult as OkObjectResult;

            // Ensure the result has data and it's of the correct type
            Assert.IsNotNull(okResult?.Value, "Expected non-null result value.");
            var inventoryList = okResult.Value as IEnumerable<InventoryStockModel>;
            Assert.IsNotNull(inventoryList, "Expected value to be of type IEnumerable<InventoryModel>.");

            // Check for no duplicates in the inventory list based on SKU
            var allUnique = inventoryList.Select(x => x.SKU).Distinct().Count() == inventoryList.Count();
            Assert.IsTrue(allUnique, "There should be no duplicates in the inventory stock list based on SKU.");
        }

    }
}
