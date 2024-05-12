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

        
        public async Task<(HashSet<PaginationItemModel>, int )> GetItemListAndCountPagination(string? listed, string? offerable, string? userHash, int pageNum, int pageSize)
        {
            var items = new HashSet<PaginationItemModel>();
            int totalCount = 0;
            
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string baseQuery = "";
                    var cmdParams = new List<MySqlParameter>();

                    // Construct the base query depending on the filters provided
                    if (listed == "true")
                    {
                        baseQuery = "FROM CraftItem WHERE Listed = 1";
                        if (offerable != null)
                        {
                            baseQuery += " AND OfferablePrice = @OfferablePrice";
                            cmdParams.Add(new MySqlParameter("@OfferablePrice", offerable == "true" ? 1 : 0));
                        }
                        if (userHash != null)
                        {
                            baseQuery += " AND CreatorHash = @userHash";
                            cmdParams.Add(new MySqlParameter("@userHash", userHash));
                        }
                    }
                    else if (userHash != null)
                    {
                        baseQuery = "FROM CraftItem WHERE CreatorHash = @userHash";
                        cmdParams.Add(new MySqlParameter("@userHash", userHash));
                    }
                    else
                    {
                        throw new Exception("Incorrect input, cannot make a query to access database");
                    }
                    // Query to get total count
                    string countQuery = $"SELECT COUNT(*) {baseQuery};";
                    using (MySqlCommand countCommand = new MySqlCommand(countQuery, connection))
                    {
                        countCommand.Parameters.AddRange(cmdParams.ToArray());
                        totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                    }
                    
                    // Query to get paginated items
                    string itemQuery = $"SELECT Name, Price, SKU, StockAvailable, Image {baseQuery} ORDER BY Price ASC LIMIT @Offset, @PageSize;";
                    cmdParams.Add(new MySqlParameter("@PageSize", pageSize));
                    cmdParams.Add(new MySqlParameter("@Offset", (pageNum - 1) * pageSize));

                    using (MySqlCommand itemCommand = new MySqlCommand(itemQuery, connection))
                    {
                        itemCommand.Parameters.AddRange(cmdParams.ToArray());
                        using (var reader = await itemCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new PaginationItemModel
                                {
                                    Name = reader.GetString("Name"),
                                    Price = reader.GetDecimal("Price"),
                                    Sku = reader.GetString("SKU"),
                                    StockAvailable = reader.GetInt32("StockAvailable")
                                };

                                var imageString = reader.GetString("Image");
                                var firstImageName = imageString.Split(',')[0]; // Gets the first image from string of name or null if none
                                item.FirstImage = firstImageName != null ? GetImageUrl(item.Sku, firstImageName) : null;  // Gets the imageURL from s3 bucket based on sku and file name
                                items.Add(item);
                            }
                        }
                    }
                }
                return (items, totalCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Failed to fetch items: " + ex.Message, ex);
            }
        }


        //public async Task<(HashSet<PaginationItemWithReceiptsModel>, int count)> GetItemListAndCountPaginationPendingSale(string? userHash, int pageNum, int pageSize)
        //{
        //    var items = new HashSet<PaginationItemWithReceiptsModel>();
        //    int totalCount = 0;

        //    using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    {
        //        await connection.OpenAsync();

        //        string baseQuery = @"FROM CraftItem ci
        //                     RIGHT JOIN CraftReceipt cr ON ci.SKU = cr.SKU AND cr.PendingSale = 1
        //                     WHERE ci.CreatorHash = @CreatorHash";

        //        string countQuery = $@"SELECT COUNT(DISTINCT ci.SKU) {baseQuery};";

        //        string dataQuery = $@"SELECT ci.Name, ci.SKU, ci.Price, ci.StockAvailable, ci.Image AS FirstImage,
        //                             cr.ReceiptID, cr.OfferPrice, cr.Quantity, cr.Profit, cr.Revenue, cr.SaleDate
        //                      {baseQuery}
        //                      ORDER BY ci.Name ASC, cr.SaleDate ASC
        //                      LIMIT @Offset, @PageSize;";

        //        using (MySqlCommand countCommand = new MySqlCommand(countQuery, connection))
        //        {
        //            countCommand.Parameters.AddWithValue("@CreatorHash", userHash ?? string.Empty);
        //            totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
        //        }

        //        using (MySqlCommand dataCommand = new MySqlCommand(dataQuery, connection))
        //        {
        //            dataCommand.Parameters.AddWithValue("@CreatorHash", userHash ?? string.Empty);
        //            dataCommand.Parameters.AddWithValue("@Offset", (pageNum - 1) * pageSize);
        //            dataCommand.Parameters.AddWithValue("@PageSize", pageSize);

        //            using (var reader = (MySqlDataReader)await dataCommand.ExecuteReaderAsync())
        //            {
        //                var itemDictionary = new Dictionary<string, PaginationItemWithReceiptsModel>();

        //                while (await reader.ReadAsync())
        //                {
        //                    string sku = reader.GetString("SKU");

        //                    if (!itemDictionary.TryGetValue(sku, out PaginationItemWithReceiptsModel? itemWithReceipts))
        //                    {
        //                        string firstImageField = reader.GetString("FirstImage");
        //                        List<string> imageUrls = firstImageField?.Split(',').ToList() ?? new List<string>();

        //                        itemWithReceipts = new PaginationItemWithReceiptsModel
        //                        {
        //                            Item = new PaginationItemModel
        //                            {
        //                                Name = reader.GetString("Name"),
        //                                Sku = sku,
        //                                Price = reader.GetDecimal("Price"),
        //                                StockAvailable = reader.GetInt32("StockAvailable"),
        //                                FirstImage = imageUrls.Count > 0 ? GetImageUrl(sku, imageUrls[0]) : null
        //                            },
        //                            PendingReceipts = new List<CraftReceiptModel>()
        //                        };

        //                        itemDictionary.Add(sku, itemWithReceipts);
        //                        items.Add(itemWithReceipts);
        //                    }

        //                    if (!reader.IsDBNull(reader.GetOrdinal("ReceiptID")))
        //                    {
        //                        var receipt = new CraftReceiptModel
        //                        {
        //                            ReceiptID = reader.GetInt32("ReceiptID"),
        //                            SKU = sku,
        //                            OfferPrice = reader.GetDecimal("OfferPrice"),
        //                            Quantity = reader.GetInt32("Quantity"),
        //                            Profit = reader.GetDecimal("Profit"),
        //                            Revenue = reader.GetDecimal("Revenue"),
        //                            SaleDate = reader.IsDBNull(reader.GetOrdinal("SaleDate")) ? (DateTime?)null : reader.GetDateTime("SaleDate")
        //                        };

        //                        itemWithReceipts.PendingReceipts.Add(receipt);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return (items, totalCount);
        //}


        public string? GetImageUrl(string sku, string picName) // getting s3 pic presigned url is synchonous task, no need await.
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
