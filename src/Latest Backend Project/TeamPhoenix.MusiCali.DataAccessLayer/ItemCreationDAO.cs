using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
     public class ItemCreationDAO
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;

        public ItemCreationDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            //this.connectionString = configuration.GetConnectionString("ConnectionString")!;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }

        public bool IsSkuDuplicate(string sku)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the SKU already exists in the CraftItem table
                    string checkDuplicateSql = "SELECT COUNT(*) FROM CraftItem WHERE SKU = @SKU";
                    using (MySqlCommand cmd = new MySqlCommand(checkDuplicateSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@SKU", sku);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as per your error handling policy
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;  // Depending on how you want to handle errors, you might return false or rethrow the exception
            }
        }

        public bool InsertIntoItemTable(ItemCreationModel model)
        {
            try
            {
                Console.WriteLine(model);
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // Open the MySQL connection.
                    string commandText = @"
                INSERT INTO CraftItem (Name, CreatorHash, SKU, Price, Description, StockAvailable, ProductionCost, OfferablePrice, SellerContact, Image, Video, DateCreated, Listed) 
                VALUES (@Name, @CreatorHash, @SKU, @Price, @Description, @StockAvailable, @ProductionCost, @OfferablePrice, @SellerContact, @Image, @Video, @DateCreated, @Listed);";

                    using (var command = new MySqlCommand(commandText, connection))
                    {
                        // Assign parameters to prevent SQL Injection.
                        command.Parameters.AddWithValue("@Name", model.Name);
                        command.Parameters.AddWithValue("@CreatorHash", model.CreatorHash);
                        command.Parameters.AddWithValue("@SKU", model.Sku); // Ensure this matches the property name in the model.
                        command.Parameters.AddWithValue("@Price", model.Price);
                        command.Parameters.AddWithValue("@Description", model.Description);
                        command.Parameters.AddWithValue("@StockAvailable", model.StockAvailable);
                        command.Parameters.AddWithValue("@ProductionCost", model.ProductionCost);
                        command.Parameters.AddWithValue("@OfferablePrice", model.OfferablePrice ? 1 : 0);
                        command.Parameters.AddWithValue("@SellerContact", model.SellerContact);
                        command.Parameters.AddWithValue("@Image", string.Join(",", model.ImageUrls!)); // Assuming ImageUrls is a List<string>
                        command.Parameters.AddWithValue("@Video", string.Join(",", model.VideoUrls!)); // Assuming VideoUrls is a List<string>
                        command.Parameters.AddWithValue("@DateCreated", model.DateCreated);
                        command.Parameters.AddWithValue("@Listed", model.Listed);
                        command.ExecuteNonQuery();
                        Console.WriteLine("Data inserted successfully!");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                // Rethrow the exception after logging it
                return false;
            }
        }


    }
}
