using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Amazon.S3;
using Amazon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class ItemPaginationTest
    {
        private IConfiguration _configuration;
        private IAmazonS3 _s3Client;
        private string _testConnectionString;
        private ItemPaginationDAO _itemPaginationDAO;
        public ItemPaginationTest()
        {
            // Load configuration from appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();

            // Read connection string from appsettings.json
            _testConnectionString = _configuration.GetConnectionString("ConnectionString")!;

            // Read AWS settings from appsettings.json
            var awsAccessKey = _configuration["AWS:AccessKey"];
            var awsSecretKey = _configuration["AWS:SecretKey"];
            var awsRegion = _configuration["AWS:Region"];

            _s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.GetBySystemName(awsRegion));

            _itemPaginationDAO = new ItemPaginationDAO(_s3Client, _configuration);

            SetupDatabase();
        }

        [TestCleanup]
        public void Cleanup()
        {
            CleanupTestData();
        }

        private void SetupDatabase()
        {
            using (var connection = new MySqlConnection(_testConnectionString))
            {
                connection.Open();

                var createTableQuery = @"
                CREATE TABLE IF NOT EXISTS CraftItem (
                    Name VARCHAR(255),
                    CreatorHash VARCHAR(255),
                    SKU VARCHAR(50) PRIMARY KEY,
                    Price DECIMAL(10, 2),
                    Description TEXT,
                    StockAvailable INT,
                    ProductionCost DECIMAL(10, 2),
                    OfferablePrice BOOL,
                    SellerContact VARCHAR(255),
                    Image TEXT,
                    Video TEXT,
                    DateCreated DATETIME,
                    Listed BOOL
                );";

                using (var command = new MySqlCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void CleanupTestData()
        {
            using (var connection = new MySqlConnection(_testConnectionString))
            {
                connection.Open();
                var deleteDataQuery = "DELETE FROM CraftItem WHERE SKU LIKE 'TEST%';";

                using (var command = new MySqlCommand(deleteDataQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void InsertDummyData(string sku, string creatorHash, bool listed, bool offerable)
        {
            using (var connection = new MySqlConnection(_testConnectionString))
            {
                connection.Open();
                var insertDataQuery = @"
                INSERT INTO CraftItem (Name, CreatorHash, SKU, Price, Description, StockAvailable, ProductionCost, OfferablePrice, SellerContact, Image, Video, DateCreated, Listed)
                VALUES ('Test Item', @CreatorHash, @SKU, 10.99, 'Test description', 10, 5.50, @OfferablePrice, 'contact@example.com', 'image1.png,image2.png', 'video1.mp4', @DateCreated, @Listed);";

                using (var command = new MySqlCommand(insertDataQuery, connection))
                {
                    command.Parameters.AddWithValue("@CreatorHash", creatorHash);
                    command.Parameters.AddWithValue("@SKU", sku);
                    command.Parameters.AddWithValue("@OfferablePrice", offerable ? 1 : 0);
                    command.Parameters.AddWithValue("@Listed", listed ? 1 : 0);
                    command.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }

        [TestMethod]
        public async Task GetItemListAndCountPagination_ShouldReturnCorrectItemsAndCount_WhenListedAndOfferable()
        {
            // Arrange
            var listed = "true";
            var offerable = "true";
            var userHash = "e12a8f14d3623f5206c060b0d1fba3d7105afc5062d13173aa17866d3b53b0d6";
            var pageNum = 1;
            var pageSize = 2;
            InsertDummyData("TEST58095566", userHash, listed: true, offerable: true);
            InsertDummyData("TEST58095545", userHash, listed: true, offerable: true);

            // Act
            var (items, totalCount) = await _itemPaginationDAO.GetItemListAndCountPagination(listed, offerable, userHash, pageNum, pageSize);

            // Assert
            Assert.AreEqual(2, totalCount, "Total count should be 2.");
            Assert.AreEqual(2, items.Count, "Should return 2 items.");
        }

        [TestMethod]
        public async Task GetItemListAndCountPagination_ShouldReturnOnlyListedItems_WhenListedTrueAndOfferableNull()
        {
            // Arrange
            var listed = "true";
            string? offerable = null;
            var userHash = "e12a8f14d3623f5206c060b0d1fba3d7105afc5062d13173aa17866d3b53b0d6";
            var pageNum = 1;
            var pageSize = 2;
            InsertDummyData("TESTSKU12895", userHash, listed: true, offerable: false);
            InsertDummyData("TESTSKU67790", userHash, listed: true, offerable: true);
            InsertDummyData("TESTSKU19911", userHash, listed: false, offerable: true);

            // Act
            var (items, totalCount) = await _itemPaginationDAO.GetItemListAndCountPagination(listed, offerable, userHash, pageNum, pageSize);

            // Assert
            Assert.AreEqual(2, totalCount, "Total count should be 2.");
            Assert.AreEqual(2, items.Count, "Should return 2 listed items.");
        }

        [TestMethod]
        public async Task GetItemListAndCountPagination_ShouldReturnOnlyUserItems_WhenListedNullAndUserHashProvided()
        {
            // Arrange
            string? listed = null;
            string? offerable = null;
            var userHash = "e12a8f14d3623f5206c060b0d1fba3d7105afc5062d13173aa17866d3b53b0d6";
            var pageNum = 1;
            var pageSize = 2;
            InsertDummyData("TESTSKU1hj45", userHash, listed: false, offerable: false);
            InsertDummyData("TESTSKU6ok90", userHash, listed: true, offerable: true);
            InsertDummyData("TESTSKU9tr99", "c956cbcead4edef814b8d2729e37556be9d6eb3df25b5e392d7fd386db56c8f8", listed: true, offerable: true);

            // Act
            var (items, totalCount) = await _itemPaginationDAO.GetItemListAndCountPagination(listed, offerable, userHash, pageNum, pageSize);

            // Assert
            Assert.AreEqual(2, totalCount, "Total count should be 2.");
            Assert.AreEqual(2, items.Count, "Should return 2 items for the user hash.");
        }

        [TestMethod]
        public async Task GetItemListAndCountPagination_ShouldReturnEmpty_WhenInvalidFilterCombination()
        {
            // Arrange
            string? listed = null;
            string? offerable = null;
            string? userHash = null;
            var pageNum = 1;
            var pageSize = 2;

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _itemPaginationDAO.GetItemListAndCountPagination(listed, offerable, userHash, pageNum, pageSize);
            }, "Incorrect input, cannot make a query to access database");
        }
    }
}