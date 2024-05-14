using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
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
                // Log the exception or handle it
                Console.WriteLine("An error occurred at checking duplicated sku. " + ex.Message);
                return false;  
            }
        }

        public async Task<bool> InsertIntoItemTable(ItemCreationModel model)
        {
            try
            {
                
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync(); // Open the MySQL connection.
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

        public async Task<ItemCreationModel> GetItemBySkuAsync(string sku, bool s3Pic)
        {
            ItemCreationModel? item = null;
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
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

                if (item != null && s3Pic)
                {
                    List<string> imagelist = new List<string>();
                    List<string> videolist = new List<string>();
                    foreach (var filename in item.ImageUrls!)
                    {                       
                        if(!(string.IsNullOrEmpty(filename) || string.IsNullOrWhiteSpace(filename)))
                            imagelist.Add(GetImageUrl(item.Sku!, filename)!);
                    }

                    foreach (var filename2 in item.VideoUrls!)
                    {                   
                        if (!(string.IsNullOrEmpty(filename2) || string.IsNullOrWhiteSpace(filename2)))
                            videolist.Add(GetImageUrl(item.Sku!, filename2)!);
                    }
                    item.ImageUrls.Clear();
                    item.ImageUrls = imagelist;
                    item.VideoUrls.Clear();
                    item.VideoUrls = videolist;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetItemBySku: {ex.Message}");
                throw; // Optionally rethrow if you want to handle this further up the call stack
            }

            return item!;
        }




        public string? GetImageUrl(string sku, string picName) // getting s3 pic presigned url is synchonous task, no need await.
        {
            if (string.IsNullOrEmpty(picName) || string.IsNullOrWhiteSpace(picName))
            {
                return null;
            }
            Console.WriteLine($"{picName}");
            string firstImageKey = $"{sku}/{picName}";

            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = firstImageKey,
                    Expires = DateTime.Now.AddMinutes(60) // URL valid for 60 minutes
                };

                string url = _s3Client.GetPreSignedURL(request);
                return url;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                return null;
            }
        }



        public async Task<bool> UpdateStockAvailable(string sku, int newStock)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync(); // Open the MySQL connection.
                    string commandText = "UPDATE CraftItem SET StockAvailable = @NewStock WHERE SKU = @SKU;";
                    using (var command = new MySqlCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@NewStock", newStock);
                        command.Parameters.AddWithValue("@SKU", sku);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Stock updated successfully!");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("No item found with the provided SKU.");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return false;
            }
        }

        

    }
}
