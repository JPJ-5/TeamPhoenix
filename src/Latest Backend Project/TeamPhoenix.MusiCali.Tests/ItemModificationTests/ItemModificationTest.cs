using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using MySql.Data.MySqlClient;

namespace TeamPhoenix.MusiCali.Tests.DataAccessLayer
{
    [TestClass]
    public class ItemModificationTests
    {
        private readonly IConfiguration configuration;
        private readonly ItemModificationDAO itemModificationDAO;

        public ItemModificationTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();

            itemModificationDAO = new ItemModificationDAO(configuration);
        }

        [TestMethod]
        public async Task ModifyItemTable_ShouldReturnTrue_WhenValidData()
        {
            // Arrange
            var creatorHash = "d1289abbbe75c552287c32667c829499f43620fc42626d653225711de97eff4b";
            var sku = "testmodific";
            var testItem = new ItemCreationModel
            {
                Name = "Initial Test Item",
                CreatorHash = creatorHash,
                Sku = sku,
                Price = 50m,
                Description = "Initial Test Description",
                StockAvailable = 5,
                ProductionCost = 20m,
                OfferablePrice = true,
                SellerContact = "test@seller.com",
                ImageUrls = new List<string> { "image1.jpg", "image2.jpg" },
                VideoUrls = new List<string> { "video1.mp4" },
                DateCreated = DateTime.UtcNow,
                Listed = true
            };
            await InsertTestItem(testItem);

            var modifiedItem = new ItemCreationModel
            {
                Name = "Modified Test Item",
                Sku = sku,
                Price = 100m,
                Description = "Modified Test Description",
                StockAvailable = 10,
                ProductionCost = 50m,
                OfferablePrice = false,
                SellerContact = "modified@seller.com",
                ImageUrls = new List<string> { "image3.jpg", "image4.jpg" },
                VideoUrls = new List<string> { "video2.mp4" },
                Listed = false
            };

            // Act
            var result = await itemModificationDAO.ModifyItemTable(creatorHash, modifiedItem);

            // Assert
            Assert.IsTrue(result, "The modification should return true for valid data.");
        }

        [TestMethod]
        public async Task ModifyItemTable_ShouldReturnFalse_WhenInvalidCreatorHash()
        {
            // Arrange
            var creatorHash = "eb4e92b99829441156353cb27f7897de0e0258bd15e8e583398d2b397bfb4745";
            var sku = "wowwowwowwow";
            var modifiedItem = new ItemCreationModel
            {
                Name = "Test Item",
                Sku = sku,
                Price = 100m,
                Description = "Test Description",
                StockAvailable = 10,
                ProductionCost = 50m,
                OfferablePrice = false,
                SellerContact = "test@seller.com",
                ImageUrls = new List<string> { "image3.jpg", "image4.jpg" },
                VideoUrls = new List<string> { "video2.mp4" },
                Listed = false
            };

            // Act
            var result = await itemModificationDAO.ModifyItemTable(creatorHash, modifiedItem);

            // Assert
            Assert.IsFalse(result, "The modification should return false for an invalid creator hash.");
        }

        [TestMethod]
        public async Task ModifyItemTable_ShouldReturnFalse_WhenInvalidSKU()
        {
            // Arrange
            var creatorHash = "d1289abbbe75c552287c32667c829499f43620fc42626d653225711de97eff4b";
            var sku = "testmodifiy";
            var modifiedItem = new ItemCreationModel
            {
                Name = "Test Item",
                Sku = sku,
                Price = 100m,
                Description = "Test Description",
                StockAvailable = 10,
                ProductionCost = 50m,
                OfferablePrice = false,
                SellerContact = "test@seller.com",
                ImageUrls = new List<string> { "image3.jpg", "image4.jpg" },
                VideoUrls = new List<string> { "video2.mp4" },
                Listed = false
            };

            // Act
            var result = await itemModificationDAO.ModifyItemTable(creatorHash, modifiedItem);

            // Assert
            Assert.IsFalse(result, "The modification should return false for an invalid SKU.");
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
                    command.Parameters.AddWithValue("@Listed", model.Listed ? 1 : 0);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}