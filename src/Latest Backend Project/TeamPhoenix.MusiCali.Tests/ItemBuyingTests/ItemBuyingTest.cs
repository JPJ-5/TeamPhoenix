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
    public class ItemBuyingTest
    {
        private IConfiguration _configuration;
        private IAmazonS3 _s3Client;
        private string _testConnectionString;
        private ItemBuyingDAO _itemBuyingDAO;

        public ItemBuyingTest()
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

            _itemBuyingDAO = new ItemBuyingDAO(_configuration, _s3Client);

            InsertDummyData();
        }

        [TestCleanup]
        public void Cleanup()
        {
            CleanupTestData();
        }

        private void CleanupTestData()
        {
            using (var connection = new MySqlConnection(_testConnectionString))
            {
                connection.Open();
                var deleteReceiptQuery = "DELETE FROM CraftReceipt WHERE SKU LIKE 'TEST%';";
                var deleteItemQuery = "DELETE FROM CraftItem WHERE SKU LIKE 'TEST%';";
                var deleteUserAccountQuery = "DELETE FROM UserAccount WHERE Username LIKE 'testuser%';";

                using (var command = new MySqlCommand(deleteReceiptQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new MySqlCommand(deleteItemQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new MySqlCommand(deleteUserAccountQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void InsertDummyData()
        {
            var items = new List<(string Name, string CreatorHash, string SKU, string Price, string Description, int StockAvailable, string ProductionCost, int OfferablePrice, string SellerContact, string Image, string Video, string DateCreated, int Listed)>
            {
                ("405 Freeway", "eb4e92b99829441156353cb27f7897de0e0258bd15e8e583398d2b697bfb4788", "TESTSKU12345", "200.00", "jytuytr uytr", 10, "10.00", 1, "nah htrtr", "pic2.jpg,string.jpg,test1.jpg", "video1.mp4", "2024-05-06 02:07:45", 1),
                ("check item modification", "824b95f537b9c57485be4bf4700421058446b63e17dd20634ba26715255b44d7", "TESTSKU67890", "1234.00", "333", 410, "22.00", 1, "234234", "", "", "2024-05-10 04:18:23", 1),
                ("kjnkjnjklnlkn", "d1289abbbe75c552287c32667c829499f43620fc42626d653225711de97eff4b", "TESTSKU11111", "111.00", "sckljnmlkmlmk", 0, "0.00", 0, "sclkklm", "", "", "2024-05-02 01:25:40", 0)
            };

            using (var connection = new MySqlConnection(_testConnectionString))
            {
                connection.Open();
                var insertItemQuery = @"
                INSERT INTO CraftItem (Name, CreatorHash, SKU, Price, Description, StockAvailable, ProductionCost, OfferablePrice, SellerContact, Image, Video, DateCreated, Listed)
                VALUES (@Name, @CreatorHash, @SKU, @Price, @Description, @StockAvailable, @ProductionCost, @OfferablePrice, @SellerContact, @Image, @Video, @DateCreated, @Listed);";

                foreach (var item in items)
                {
                    using (var command = new MySqlCommand(insertItemQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", item.Name);
                        command.Parameters.AddWithValue("@CreatorHash", item.CreatorHash);
                        command.Parameters.AddWithValue("@SKU", item.SKU);
                        command.Parameters.AddWithValue("@Price", item.Price);
                        command.Parameters.AddWithValue("@Description", item.Description);
                        command.Parameters.AddWithValue("@StockAvailable", item.StockAvailable);
                        command.Parameters.AddWithValue("@ProductionCost", item.ProductionCost);
                        command.Parameters.AddWithValue("@OfferablePrice", item.OfferablePrice);
                        command.Parameters.AddWithValue("@SellerContact", item.SellerContact);
                        command.Parameters.AddWithValue("@Image", item.Image);
                        command.Parameters.AddWithValue("@Video", item.Video);
                        command.Parameters.AddWithValue("@DateCreated", DateTime.Parse(item.DateCreated));
                        command.Parameters.AddWithValue("@Listed", item.Listed);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        [TestMethod]
        public async Task InsertRecieptTable_ShouldReturnTrue_WhenDataInsertedSuccessfully()
        {
            // Arrange
            var receipt = new CraftReceiptModel
            {
                CreatorHash = "eb4e92b99829441156353cb27f7897de0e0258bd15e8e583398d2b697bfb4788",
                BuyerHash = "d1289abbbe75c552287c32667c829499f43620fc42626d653225711de97eff4b",
                SKU = "TESTSKU12345",
                SellPrice = 200.00m,
                OfferPrice = 180.00m,
                Profit = 20.00m,
                Revenue = 200.00m,
                Quantity = 1,
                SaleDate = DateTime.Now,
                PendingSale = true
            };

            // Act
            var result = await _itemBuyingDAO.InsertRecieptTable(receipt);

            // Assert
            Assert.IsTrue(result, "Receipt should be inserted successfully.");
        }

        [TestMethod]
        public void GetEmailByUserHash_ShouldReturnCorrectEmail_WhenUserHashExists()
        {
            // Arrange
            var userHash = "eb4e92b99829441156353cb27f7897de0e0258bd15e8e583398d2b697bfb4788";
            var expectedEmail = "creator1@example.com";

            // Act
            var result = _itemBuyingDAO.GetEmailByUserHash(userHash);

            // Assert
            Assert.AreEqual(expectedEmail, result, "Email should match the expected value.");
        }

        [TestMethod]
        public void GetUserHashBySku_ShouldReturnCorrectUserHash_WhenSkuExists()
        {
            // Arrange
            var sku = "TESTSKU12345";
            var expectedUserHash = "eb4e92b99829441156353cb27f7897de0e0258bd15e8e583398d2b697bfb4788";

            // Act
            var result = _itemBuyingDAO.GetUserHashBySku(sku);

            // Assert
            Assert.AreEqual(expectedUserHash, result, "User hash should match the expected value.");
        }

        [TestMethod]
        public async Task AcceptPendingSale_ShouldReturnTrue_WhenSaleAcceptedSuccessfully()
        {
            // Arrange
            var receipt = new CraftReceiptModel
            {
                ReceiptID = 1,
                CreatorHash = "eb4e92b99829441156353cb27f7897de0e0258bd15e8e583398d2b697bfb4788",
                BuyerHash = "d1289abbbe75c552287c32667c829499f43620fc42626d653225711de97eff4b",
                SKU = "TESTSKU12345",
                SellPrice = 200.00m,
                OfferPrice = 180.00m,
                Profit = 20.00m,
                Revenue = 200.00m,
                Quantity = 1,
                SaleDate = DateTime.Now,
                PendingSale = true
            };

            await _itemBuyingDAO.InsertRecieptTable(receipt);

            // Act
            var result = await _itemBuyingDAO.AcceptPendingSale(receipt);

            // Assert
            Assert.IsTrue(result, "Sale should be accepted successfully.");
        }

        [TestMethod]
        public async Task DeclinePendingSale_ShouldReturnTrue_WhenSaleDeclinedSuccessfully()
        {
            // Arrange
            var receipt = new CraftReceiptModel
            {
                ReceiptID = 1,
                CreatorHash = "eb4e92b99829441156353cb27f7897de0e0258bd15e8e583398d2b697bfb4788",
                BuyerHash = "d1289abbbe75c552287c32667c829499f43620fc42626d653225711de97eff4b",
                SKU = "TESTSKU12345",
                SellPrice = 200.00m,
                OfferPrice = 180.00m,
                Profit = 20.00m,
                Revenue = 200.00m,
                Quantity = 1,
                SaleDate = DateTime.Now,
                PendingSale = true
            };

            await _itemBuyingDAO.InsertRecieptTable(receipt);

            // Act
            var result = await _itemBuyingDAO.DeclinePendingSale(receipt.ReceiptID);

            // Assert
            Assert.IsTrue(result, "Sale should be declined successfully.");
        }

        [TestMethod]
        public async Task GetPendingReceiptsWithItemInfo_ShouldReturnCorrectReceipts_WhenUserHashProvided()
        {
            // Arrange
            var userHash = "eb4e92b99829441156353cb27f7897de0e0258bd15e8e583398d2b697bfb4788";
            var pageNum = 1;
            var pageSize = 2;

            var receipt = new CraftReceiptModel
            {
                CreatorHash = userHash,
                BuyerHash = "d1289abbbe75c552287c32667c829499f43620fc42626d653225711de97eff4b",
                SKU = "TESTSKU12345",
                SellPrice = 200.00m,
                OfferPrice = 180.00m,
                Profit = 20.00m,
                Revenue = 200.00m,
                Quantity = 1,
                SaleDate = DateTime.Now,
                PendingSale = true
            };

            await _itemBuyingDAO.InsertRecieptTable(receipt);

            // Act
            var (receipts, totalCount) = await _itemBuyingDAO.GetPendingReceiptsWithItemInfo(userHash, pageNum, pageSize);

            // Assert
            Assert.AreEqual(1, totalCount, "Total count should be 1.");
            Assert.AreEqual(1, receipts.Count, "Should return 1 receipt.");
        }
    }
}