using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Configuration;

namespace MyApp.Tests
{
    [TestClass]
    public class ItemSortingTests
    {
        // Test dependencies: configuration and data access layer
        private readonly IConfiguration configuration;
        private readonly DataAccessLayer dal;
        private IAmazonS3 s3Client;

        // Constructor to initialize and configure test dependencies
        public ItemSortingTests()
        {
            // Set up configuration for the application base path
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            // Retrieve AWS credentials and region from the configuration
            var awsAccessKey = configuration["AWS:AccessKey"];
            var awsSecretKey = configuration["AWS:SecretKey"];
            var awsRegion = configuration["AWS:Region"];

            // Initialize the S3 client with the retrieved values
            s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.GetBySystemName(awsRegion));

            // Initialize the data access layer (without S3)
            dal = new DataAccessLayer(configuration, s3Client);
        }

        // Test to ensure all items are returned when no filters are applied
        [TestMethod]
        public async Task FetchPagedItems_ReturnsAllItemsIfNoFilterApplied()
        {
            var (items, _) = await dal.FetchPagedItems(1, 10);
            Assert.IsTrue(items.Count > 0, "Should return items without any filter.");
        }

        // Test to verify that items returned contain the specified name filter
        [TestMethod]
        public async Task FetchPagedItems_ReturnsFilteredItemsByName()
        {
            var (items, _) = await dal.FetchPagedItems(1, 10, name: "craft");
            Assert.IsTrue(items.All(item => item.Name!.ToLower().Contains("craft")), "All items should contain the name 'craft'.");
        }

        // Test to verify that items are returned within the specified price range
        [TestMethod]
        public async Task FetchPagedItems_ReturnsItemsWithinSpecifiedPriceRange()
        {
            var bottomPrice = 50m;
            var topPrice = 100m;
            var (items, _) = await dal.FetchPagedItems(1, 10, bottomPrice: bottomPrice, topPrice: topPrice);
            Assert.IsTrue(items.All(item => item.Price >= bottomPrice && item.Price <= topPrice), "All items should be within the specified price range.");
        }

        // Test to ensure items returned match both name and price range filters
        [TestMethod]
        public async Task FetchPagedItems_ReturnsFilteredItemsByNameAndPriceRange()
        {
            var (items, _) = await dal.FetchPagedItems(1, 10, name: "craft", bottomPrice: 10m, topPrice: 1000m);
            Assert.IsTrue(items.All(item => item.Name!.ToLower().Contains("craft") && item.Price >= 10m && item.Price <= 1000m), "All items should match the name 'craft' and be within the price range.");
        }

        // Test to ensure the total item count is greater than zero
        [TestMethod]
        public async Task CountItems_ReturnsTotalItemCount()
        {
            var totalCount = await dal.CountItems();
            Assert.IsTrue(totalCount > 0, "Total item count should be greater than zero.");
        }
    }
}