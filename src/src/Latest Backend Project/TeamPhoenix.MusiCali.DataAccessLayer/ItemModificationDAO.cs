
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System.Data.SqlClient;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;

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

        public async Task<bool> ModifyItemTable(string creatorHash, ItemCreationModel model)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync(); // Open the MySQL connection.

                    string commandText = "UPDATE CraftItem SET OfferablePrice = @OfferablePrice, Listed = @Listed";

                    if (!(string.IsNullOrWhiteSpace(model.Name)))
                    {
                        commandText += ", Name = @Name";
                    }
                    if (model.Price >= 0m && model.Price <= 1000000m)
                    {
                        commandText += ", Price = @Price";
                    }
                    if (!string.IsNullOrWhiteSpace(model.Description))
                    {
                        commandText += ", Description = @Description";
                    }
                    if (model.StockAvailable >= 0)
                    {
                        commandText += ", StockAvailable = @StockAvailable";
                    }
                    if (model.ProductionCost >= 0 && model.ProductionCost <= 1000000m)
                    {
                        commandText += ", ProductionCost = @ProductionCost";
                    }
                    if (!string.IsNullOrWhiteSpace(model.SellerContact))
                    {
                        commandText += ", SellerContact = @SellerContact";
                    }
                    if ( model.ImageUrls != null && model.ImageUrls.Any() && model.ImageUrls.All(s => !string.IsNullOrWhiteSpace(s)))                             //image is not null here but whitespace or enter condition is bad
                    {
                        commandText += ", Image = @Image";
                    }
                    if (model.VideoUrls != null && model.VideoUrls.Any() && model.VideoUrls.All(s => !string.IsNullOrWhiteSpace(s)))                        // video is also null 
                    {
                        commandText += ", Video = @Video";
                    }
                    commandText += " WHERE SKU = @SKU AND CreatorHash = @CreatorHash";


                    using (var command = new MySqlCommand(commandText, connection))
                    {
                        // Assign parameters to prevent SQL Injection.
                        command.Parameters.AddWithValue("@OfferablePrice", model.OfferablePrice);
                        command.Parameters.AddWithValue("@Listed", model.Listed);
                        command.Parameters.AddWithValue("@SKU", model.Sku);
                        command.Parameters.AddWithValue("@CreatorHash", creatorHash);



                        if (!(string.IsNullOrWhiteSpace(model.Name)))
                        {
                            command.Parameters.AddWithValue("@Name", model.Name);
                        }
                        if (model.Price >= 0m && model.Price <= 1000000m)
                        {
                            command.Parameters.AddWithValue("@Price", model.Price);
                        }
                        if (!string.IsNullOrWhiteSpace(model.Description))
                        {
                            command.Parameters.AddWithValue("@Description", model.Description);
                        }
                        if (model.StockAvailable >= 0)
                        {
                            command.Parameters.AddWithValue("@StockAvailable", model.StockAvailable);
                        }
                        if (model.ProductionCost >= 0 && model.ProductionCost <= 1000000m)
                        {
                            command.Parameters.AddWithValue("@ProductionCost", model.ProductionCost);
                        }
                        if (!string.IsNullOrWhiteSpace(model.SellerContact))
                        {
                            command.Parameters.AddWithValue("@SellerContact", model.SellerContact);
                        }
                        if (model.ImageUrls != null )
                        {
                            command.Parameters.AddWithValue("@Image", string.Join(",", model.ImageUrls));
                        }
                        if (model.VideoUrls != null )
                        {
                            command.Parameters.AddWithValue("@Video", string.Join(",", model.VideoUrls));
                        }
                        int check = await command.ExecuteNonQueryAsync();

                        if (check == 1)
                        {
                            return true;
                        }
                        else
                        {
                            throw new Exception("No rows were affected.");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Log MySQL exception
                Console.WriteLine($"MySQL error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log general exception
                Console.WriteLine($"General error: {ex.Message}");
            }

            return false;
        }


        public async Task<bool> checkItemOwner(string creatorHash, string sku)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync(); // Open the MySQL connection.
                    string commandText = "SELECT CreatorHash FROM CraftItem WHERE SKU = @SKU";
                    using (MySqlCommand command = new MySqlCommand(commandText, connection))
                    {
                        // Assign parameters to prevent SQL Injection.
                        command.Parameters.AddWithValue("@SKU", sku);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string ownerHash = reader.GetString(0);
                                if (creatorHash == ownerHash)
                                {
                                    return true;
                                }
                            }
                            throw new Exception();
                        }
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }
            catch (Exception)
            {
                //  logging it
                return false;
            }
        }
    }
}