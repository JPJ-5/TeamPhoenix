using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{
    [TestClass]
    public class ItemDeletionTests
    {
        private readonly IConfiguration configuration;
        private ItemDeletionDAO itemDeletionDAO;

        public ItemDeletionTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            itemDeletionDAO = new ItemDeletionDAO(configuration);
        }


        [TestMethod]
        public async Task DeleteItem_ShouldReturnTrue_WhenValidUserHashAndSKU()
        {
            // Arrange
            var userHash = "d1289abbbe75c552287c32667c829499f43620fc42626d653225711de97eff4b";
            var sku = "testdeletion";
            var testItem = new ItemCreationModel
            {
                Name = "Test Item",
                CreatorHash = userHash,
                Sku = sku,
                Price = 100m,
                Description = "A test item for deletion testing",
                StockAvailable = 10,
                ProductionCost = 50m,
                OfferablePrice = true,
                SellerContact = "test@seller.com",
                ImageUrls = new List<string> { "image1.jpg", "image2.jpg" },
                VideoUrls = new List<string> { "video1.mp4", "video2.mp4" },
                DateCreated = DateTime.UtcNow,
                Listed = true
            };

            // Insert a test item into the database (matching the InsertIntoItemTable method)
            await InsertTestItem(testItem);

            // Act
            var result = await itemDeletionDAO.DeleteItem(userHash, sku);

            // Assert
            Assert.IsTrue(result, "Item should be successfully deleted.");
        }


        [TestMethod]
        public async Task DeleteItem_ShouldReturnFalse_WhenInvalidUserHash()
        {
            // Arrange
            var userHash = "";
            var sku = "VhsVWQM4DPYZ";

            // Act
            var result = await itemDeletionDAO.DeleteItem(userHash, sku);

            // Assert
            Assert.IsFalse(result, "Empty user hash should result in failure.");
        }

        [TestMethod]
        public async Task DeleteItem_ShouldReturnFalse_WhenInvalidSKU()
        {
            // Arrange
            var userHash = "d1289abbbe75c552287c32667c829499f43620fc42626d653225711de97eff4b";
            var sku = "";

            // Act
            var result = await itemDeletionDAO.DeleteItem(userHash, sku);

            // Assert
            Assert.IsFalse(result, "Empty SKU should result in failure.");
        }

        private async Task InsertTestItem(ItemCreationModel model)
        {
            using (var connection = new MySqlConnection(configuration.GetSection("ConnectionStrings:ConnectionString").Value))
            {
                await connection.OpenAsync();
                string commandText = @"
                INSERT INTO CraftItem (Name, CreatorHash, SKU, Price, Description, StockAvailable, ProductionCost, OfferablePrice, SellerContact, Image, Video, DateCreated, Listed) 
                VALUES (@Name, @CreatorHash, @SKU, @Price, @Description, @StockAvailable, @ProductionCost, @OfferablePrice, @SellerContact, @Image, @Video, @DateCreated, @Listed);";

                using (var command = new MySqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@Name", model.Name);
                    command.Parameters.AddWithValue("@CreatorHash", model.CreatorHash);
                    command.Parameters.AddWithValue("@SKU", model.Sku);
                    command.Parameters.AddWithValue("@Price", model.Price);
                    command.Parameters.AddWithValue("@Description", model.Description);
                    command.Parameters.AddWithValue("@StockAvailable", model.StockAvailable);
                    command.Parameters.AddWithValue("@ProductionCost", model.ProductionCost);
                    command.Parameters.AddWithValue("@OfferablePrice", model.OfferablePrice ? 1 : 0);
                    command.Parameters.AddWithValue("@SellerContact", model.SellerContact);
                    command.Parameters.AddWithValue("@Image", string.Join(",", model.ImageUrls!));
                    command.Parameters.AddWithValue("@Video", string.Join(",", model.VideoUrls!));
                    command.Parameters.AddWithValue("@DateCreated", model.DateCreated);
                    command.Parameters.AddWithValue("@Listed", model.Listed);
                    await command.ExecuteNonQueryAsync();
                }
            }
        } 
    }
}