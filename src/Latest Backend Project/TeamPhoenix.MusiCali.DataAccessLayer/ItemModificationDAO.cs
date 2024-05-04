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
    public class ItemModificationDAO
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;

        public ItemModificationDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }


        public bool ModifyItemTable(ItemModificationModel model)
        {
            try
            {
                Console.WriteLine(model);
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // Open the MySQL connection.
                    string commandText = @"
                UPDATE INTO CraftItem (Name, Price, Description, StockAvailable, ProductionCost, OfferablePrice, SellerContact, Image, Video, DateCreated) 
                VALUES (@Name, @Price, @Description, @StockAvailable, @ProductionCost, @OfferablePrice, @SellerContact, @Image, @Video, @DateCreated);";

                    using (var command = new MySqlCommand(commandText, connection))
                    {
                        // Assign parameters to prevent SQL Injection.
                        command.Parameters.AddWithValue("@Name", model.Name);
                        command.Parameters.AddWithValue("@Price", model.Price);
                        command.Parameters.AddWithValue("@Description", model.Description);
                        command.Parameters.AddWithValue("@StockAvailable", model.StockAvailable);
                        command.Parameters.AddWithValue("@ProductionCost", model.ProductionCost);
                        command.Parameters.AddWithValue("@OfferablePrice", model.OfferablePrice ? 1 : 0);
                        command.Parameters.AddWithValue("@SellerContact", model.SellerContact);
                        command.Parameters.AddWithValue("@Image", string.Join(",", model.ImageUrls)); // Assuming ImageUrls is a List<string>
                        command.Parameters.AddWithValue("@Video", string.Join(",", model.VideoUrls)); // Assuming VideoUrls is a List<string>
                        command.Parameters.AddWithValue("@DateModified", model.Datemodified);

                        command.ExecuteNonQuery();
                        Console.WriteLine("Data inserted successfully!");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                //  logging it
                return false;
            }
        }


    }
}