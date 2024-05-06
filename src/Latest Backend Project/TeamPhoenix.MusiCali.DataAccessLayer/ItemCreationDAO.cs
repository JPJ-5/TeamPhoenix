using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Amazon.S3;



namespace TeamPhoenix.MusiCali.DataAccessLayer
{
     public class ItemCreationDAO
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;
        private readonly IAmazonS3 _s3Client;
        private readonly string bucketName;


        //public ItemCreationDAO(IConfiguration configuration, IAmazonS3 s3Client)
        public ItemCreationDAO(IConfiguration configuration, IAmazonS3 s3Client)
        {
            this.configuration = configuration;
            this.connectionString = configuration.GetConnectionString("ConnectionString")!;
            bucketName = configuration.GetValue<string>("AWS:BucketName");
            _s3Client = s3Client;
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

        public async Task<ItemCreationModel> GetItemBySkuAsync(string sku)
        {
            ItemCreationModel? item = null;
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                    SELECT Name, SKU, Price, Description, StockAvailable, ProductionCost, 
                           OfferablePrice, SellerContact, Image, Video, DateCreated, Listed
                    FROM CraftItem
                    WHERE SKU = @SKU;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SKU", sku);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                item = new ItemCreationModel
                                {
                                    Name = reader["Name"].ToString()!,
                                    Sku = reader["SKU"].ToString(),
                                    Price = reader.GetDecimal("Price"),
                                    Description = reader["Description"].ToString()!,
                                    StockAvailable = reader.GetInt32("StockAvailable"),
                                    ProductionCost = reader.GetDecimal("ProductionCost"),
                                    OfferablePrice = reader.GetBoolean("OfferablePrice"),
                                    SellerContact = reader["SellerContact"].ToString()!,
                                    ImageUrls = reader["Image"].ToString()!.Split(',').ToList(),
                                    VideoUrls = reader["Video"].ToString()!.Split(',').ToList(),
                                    DateCreated = reader.GetDateTime("DateCreated"),
                                    Listed = reader.GetBoolean("Listed")
                                };
                            }
                        }
                    }
                }

                if (item != null)
                {
                    //var s3Service = new S3Service(_s3Client, "your-bucket-name");
                    item.ImageUrls = await GeneratePreSignedURLs(item.ImageUrls!);
                    item.VideoUrls = await GeneratePreSignedURLs(item.VideoUrls!);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetItemBySku: {ex.Message}");
                throw; // Optionally rethrow if you want to handle this further up the call stack
            }

            return item!;
        }



        public Task<string> GeneratePreSignedURL(string objectKey)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.AddMinutes(10) // URL expires in 10 minutes
            };

            return Task.FromResult(_s3Client.GetPreSignedURL(request));
        }

        public async Task<List<string>> GeneratePreSignedURLs(List<string> objectKeys)
        {
            var urls = new List<string>();
            foreach (var key in objectKeys)
            {
                urls.Add(await GeneratePreSignedURL(key));
            }
            return urls;
        }

    }
}
