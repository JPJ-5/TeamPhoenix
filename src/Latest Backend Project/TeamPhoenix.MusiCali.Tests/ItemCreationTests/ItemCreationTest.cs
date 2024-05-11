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
    public class ItemCreationTest
    {
        private IConfiguration _configuration;
        private IAmazonS3 _s3Client;
        private string _testConnectionString;
        private ItemCreationDAO _itemCreationDAO;

        public ItemCreationTest()
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

            _itemCreationDAO = new ItemCreationDAO(_configuration, _s3Client);

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

        private void InsertDummyData(string sku)
        {
            using (var connection = new MySqlConnection(_testConnectionString))
            {
                connection.Open();
                var insertDataQuery = @"
                INSERT INTO CraftItem (Name, CreatorHash, SKU, Price, Description, StockAvailable, ProductionCost, OfferablePrice, SellerContact, Image, Video, DateCreated, Listed)
                VALUES ('Test Item', '824b95f537b9c57485be4bf4700421058446b63e17dd20634ba26715255b44d7', @SKU, 10.99, 'Test description', 10, 5.50, true, 'contact@example.com', 'image1.png,image2.png', 'video1.mp4', @DateCreated, true);";

                using (var command = new MySqlCommand(insertDataQuery, connection))
                {
                    command.Parameters.AddWithValue("@SKU", sku);
                    command.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }

        [TestMethod]
        public void IsSkuDuplicate_ShouldReturnTrue_WhenSkuExists()
        {
            // Arrange
            var existingSku = "TEST12345";
            InsertDummyData(existingSku);

            // Act
            var result = _itemCreationDAO.IsSkuDuplicate(existingSku);

            // Assert
            Assert.IsTrue(result, "SKU should be marked as duplicate.");
        }

        [TestMethod]
        public void IsSkuDuplicate_ShouldReturnFalse_WhenSkuDoesNotExist()
        {
            // Arrange
            var nonExistentSku = "TEST99999";

            // Act
            var result = _itemCreationDAO.IsSkuDuplicate(nonExistentSku);

            // Assert
            Assert.IsFalse(result, "SKU should not be marked as duplicate.");
        }

        [TestMethod]
        public async Task InsertIntoItemTable_ShouldReturnTrue_WhenDataInsertedSuccessfully()
        {
            // Arrange
            var newItem = new ItemCreationModel
            {
                Name = "Test Item",
                CreatorHash = "824b95f537b9c57485be4bf4700421058446b63e17dd20634ba26715255b44d7",
                Sku = "TESTSKU12345",
                Price = 10.99m,
                Description = "Test description",
                StockAvailable = 10,
                ProductionCost = 5.50m,
                OfferablePrice = true,
                SellerContact = "contact@example.com",
                ImageUrls = new List<string> { "image1.png", "image2.png" },
                VideoUrls = new List<string> { "video1.mp4" },
                DateCreated = DateTime.Now,
                Listed = true
            };

            // Act
            var result = await _itemCreationDAO.InsertIntoItemTable(newItem);

            // Assert
            Assert.IsTrue(result, "Item should be inserted successfully.");
        }

        [TestMethod]
        public async Task GetItemBySkuAsync_ShouldReturnCorrectItem_WhenItemExists()
        {
            // Arrange
            var existingSku = "TESTSKU12345";
            var expectedItem = new ItemCreationModel
            {
                Name = "Test Item",
                CreatorHash = "824b95f537b9c57485be4bf4700421058446b63e17dd20634ba26715255b44d7",
                Sku = existingSku,
                Price = 10.99m,
                Description = "Test description",
                StockAvailable = 10,
                ProductionCost = 5.50m,
                OfferablePrice = true,
                SellerContact = "contact@example.com",
                ImageUrls = new List<string> { "image1.png", "image2.png" },
                VideoUrls = new List<string> { "video1.mp4" },
                DateCreated = DateTime.Now,
                Listed = true
            };

            InsertDummyData(expectedItem.Sku!);

            // Act
            var result = await _itemCreationDAO.GetItemBySkuAsync(existingSku, false);

            // Assert
            Assert.AreEqual(expectedItem.Sku, result.Sku, "Item should have the same SKU.");
            Assert.AreEqual(expectedItem.Name, result.Name, "Item should have the same name.");
        }

        [TestMethod]
        public async Task GetItemBySkuAsync_ShouldReturnNull_WhenItemDoesNotExist()
        {
            // Arrange
            var nonExistentSku = "TEST99kjnio9";

            // Act
            var result = await _itemCreationDAO.GetItemBySkuAsync(nonExistentSku, false);

            // Assert
            Assert.IsNull(result, "Should return null if the item does not exist.");
        }

        [TestMethod]
        public async Task UpdateStockAvailable_ShouldReturnTrue_WhenStockIsUpdated()
        {
            // Arrange
            var existingSku = "TESTSKU12345";
            InsertDummyData(existingSku);
            var newStock = 15;

            // Act
            var result = await _itemCreationDAO.UpdateStockAvailable(existingSku, newStock);

            // Assert
            Assert.IsTrue(result, "Stock should be updated successfully.");
        }

        [TestMethod]
        public async Task UpdateStockAvailable_ShouldReturnFalse_WhenSkuDoesNotExist()
        {
            // Arrange
            var nonExistentSku = "TESTth57gkmk";
            var newStock = 15;

            // Act
            var result = await _itemCreationDAO.UpdateStockAvailable(nonExistentSku, newStock);

            // Assert
            Assert.IsFalse(result, "Should return false when SKU does not exist.");
        }
    }
}