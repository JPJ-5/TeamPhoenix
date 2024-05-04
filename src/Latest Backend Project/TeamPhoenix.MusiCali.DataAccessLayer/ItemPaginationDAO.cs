using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class ItemPaginationDAO
    {
        private readonly string connectionString;
        private readonly IAmazonS3 _s3Client;
        private readonly string bucketName;
        

        public ItemPaginationDAO(IAmazonS3 s3Client, IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("ConnectionString")!;
            bucketName = configuration.GetValue<string>("AWS:BucketName");
            _s3Client = s3Client;

        }

        public async Task<HashSet<PaginationItemModel>> GetItemList(string? listed, string? offerable, string? userHash)
        {
            var items = new HashSet<PaginationItemModel>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string query = "";
                    var cmdParams = new List<MySqlParameter>();

                    if (listed == "true")
                    {
                        query = "SELECT Name, Price, SKU, Image FROM CraftItem WHERE Listed = 1";

                        if (offerable != null)
                        {
                            query += " AND OfferablePrice = @OfferablePrice";
                            cmdParams.Add(new MySqlParameter("@OfferablePrice", offerable == "true" ? 1 : 0));
                        }

                        if (userHash != null)
                        {
                            query += " AND CreatorHash = @userHash";
                            cmdParams.Add(new MySqlParameter("@userHash", userHash));
                        }

                        query += " ORDER BY Price ASC;";
                    }
                    else if (userHash != null)
                    {
                        query = "SELECT Name, Price, SKU, Image FROM CraftItem WHERE CreatorHash = @userHash ORDER BY Price ASC;";
                        cmdParams.Add(new MySqlParameter("@userHash", userHash));
                    }
                    else
                    {
                        throw new Exception("Incorrect input, cannot make a query to access database");
                    }

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(cmdParams.ToArray());

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var imageString = reader.GetString("Image");
                                var firstImageName = imageString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                .Select(s => s.Trim())
                                                                .FirstOrDefault(); // Gets the first image or null if none

                                var item = new PaginationItemModel
                                {
                                    Name = reader.GetString("Name"),
                                    Price = reader.GetDecimal("Price"),
                                    Sku = reader.GetString("SKU"),
                                    
                                };
                                item.FirstImage = firstImageName != null ? GetImageUrl(item.Sku, firstImageName) : null;
                                items.Add(item);
                            }
                        }
                    }
                }
                return items;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch items: " + ex.Message, ex);
            }
        }




        public  string? GetImageUrl(string sku, string picName) // getting s3 pic presigned url is synchonous task, no need await.
        {
            if (picName == null )
            {
                return null;
            }

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

       
    }
}
