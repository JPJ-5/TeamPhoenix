using Microsoft.Extensions.Configuration;
using Amazon.S3;

namespace MyApp.Tests
{
    [TestClass]
    public class ItemSortingTests
    {
        // Test dependencies: configuration, data access layer, service layer, and S3 client
        private readonly IConfiguration configuration;
        private readonly DataAccessLayer dal;
        private readonly ItemService service;
        private readonly IAmazonS3 _s3Client;

        // Constructor to initialize and configure test dependencies
        public ItemSortingTests()
        {
            // Set up configuration for the application base path
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            // Setup the AWS S3 client using AWS options from the configuration
            var awsOptions = configuration.GetAWSOptions();
            _s3Client = awsOptions.CreateServiceClient<IAmazonS3>();

            // Initialize data access and service layers
            dal = new DataAccessLayer(configuration, _s3Client);
            service = new ItemService(dal, configuration);
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