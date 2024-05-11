using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.S3;
using System;
using Amazon;

namespace MyApp.Tests
{
    [TestClass]
    public class ItemSortingTests
    {
        private readonly IConfiguration configuration;
        private readonly DataAccessLayer dal;
        private readonly IAmazonS3 s3Client;

        public ItemSortingTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            var awsAccessKey = configuration["AWS:AccessKey"];
            var awsSecretKey = configuration["AWS:SecretKey"];
            var awsRegion = configuration["AWS:Region"];
            s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.GetBySystemName(awsRegion));

            dal = new DataAccessLayer(configuration, s3Client);
        }

        [TestMethod]
        public async Task FetchPagedItems_ReturnsAllItemsIfNoFilterApplied()
        {
            // Arrange
            var query = new ItemQueryParameters { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await dal.FetchPagedItems(query);
            var (items, _) = result;

            // Assert
            Assert.IsTrue(items.Count > 0, "Should return items without any filter.");
        }

        [TestMethod]
        public async Task FetchPagedItems_ReturnsFilteredItemsByName()
        {
            // Arrange
            var query = new ItemQueryParameters { PageNumber = 1, PageSize = 10, Name = "craft" };

            // Act
            var result = await dal.FetchPagedItems(query);
            var (items, _) = result;

            // Assert
            Assert.IsTrue(items.All(item => item.Name!.ToLower().Contains("craft")), "All items should contain the name 'craft'.");
        }

        [TestMethod]
        public async Task FetchPagedItems_ReturnsItemsWithinSpecifiedPriceRange()
        {
            // Arrange
            var query = new ItemQueryParameters { PageNumber = 1, PageSize = 10, BottomPrice = 50m, TopPrice = 100m };

            // Act
            var result = await dal.FetchPagedItems(query);
            var (items, _) = result;

            // Assert
            Assert.IsTrue(items.All(item => item.Price >= 50m && item.Price <= 100m), "All items should be within the specified price range.");
        }

        [TestMethod]
        public async Task FetchPagedItems_ReturnsFilteredItemsByNameAndPriceRange()
        {
            // Arrange
            var query = new ItemQueryParameters { PageNumber = 1, PageSize = 10, Name = "craft", BottomPrice = 10m, TopPrice = 1000m };

            // Act
            var result = await dal.FetchPagedItems(query);
            var (items, _) = result;

            // Assert
            Assert.IsTrue(items.All(item => item.Name!.ToLower().Contains("craft") && item.Price >= 10m && item.Price <= 1000m), "All items should match the name 'craft' and be within the price range.");
        }

        [TestMethod]
        public async Task CountItems_ReturnsTotalItemCount()
        {
            // Arrange & Act
            var totalCount = await dal.CountItems();

            // Assert
            Assert.IsTrue(totalCount > 0, "Total item count should be greater than zero.");
        }
    }
}