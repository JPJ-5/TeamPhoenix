using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;
using Amazon.S3.Model;
using Amazon.S3;

public class DataAccessLayer
{
    private readonly string connectionString;
    private readonly IAmazonS3 _s3Client;
    private readonly string bucketName;

    public DataAccessLayer(IConfiguration configuration, IAmazonS3 s3Client)
    {
        this.connectionString = configuration.GetConnectionString("ConnectionString")!;
        bucketName = configuration.GetValue<string>("AWS:BucketName");
        _s3Client = s3Client;
    }

    public async Task<(HashSet<Item> items, int totalCount)> FetchPagedItems(int pageNumber, int pageSize, string? name = null, decimal? bottomPrice = null, decimal? topPrice = null)
    {
        var items = new HashSet<Item>();
        int totalCount = 0;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var offset = (pageNumber - 1) * pageSize;
            
            // Base query for filtering
            var baseFilter = new StringBuilder("FROM CraftItem WHERE Listed = 1");
            
            var conditions = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(name))
            {
                conditions.Add("LOWER(Name) LIKE CONCAT(@nameQuery, '%')");
            }
            if (bottomPrice.HasValue && topPrice.HasValue)
            {
                conditions.Add("Price BETWEEN @bottomPrice AND @topPrice");
            }

            if (conditions.Count > 0)
            {
                baseFilter.Append(" AND ");
                baseFilter.Append(string.Join(" AND ", conditions));
            }

            // Query to count the total filtered items
            var countQuery = $"SELECT COUNT(*) {baseFilter}";
            
            // Query to fetch the paginated data
            var dataQuery = new StringBuilder("SELECT Name, Price, SKU, Image ");
            dataQuery.Append(baseFilter);
            dataQuery.Append(" ORDER BY Price ASC LIMIT @pageSize OFFSET @offset");

            // Execute the count query
            using (MySqlCommand countCommand = new MySqlCommand(countQuery, connection))
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    countCommand.Parameters.AddWithValue("@nameQuery", name.ToLower());
                }
                if (bottomPrice.HasValue)
                {
                    countCommand.Parameters.AddWithValue("@bottomPrice", bottomPrice.Value);
                }
                if (topPrice.HasValue)
                {
                    countCommand.Parameters.AddWithValue("@topPrice", topPrice.Value);
                }

                totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
            }

            // Execute the paginated data query
            using (MySqlCommand dataCommand = new MySqlCommand(dataQuery.ToString(), connection))
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    dataCommand.Parameters.AddWithValue("@nameQuery", name.ToLower());
                }
                if (bottomPrice.HasValue)
                {
                    dataCommand.Parameters.AddWithValue("@bottomPrice", bottomPrice.Value);
                }
                if (topPrice.HasValue)
                {
                    dataCommand.Parameters.AddWithValue("@topPrice", topPrice.Value);
                }
                dataCommand.Parameters.AddWithValue("@pageSize", pageSize);
                dataCommand.Parameters.AddWithValue("@offset", offset);

                using (var reader = await dataCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var imageString = reader.GetString("Image");
                        var firstImageName = imageString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                        items.Add(new Item
                        {
                            Name = reader.GetString("Name"),
                            Price = reader.GetDecimal("Price"),
                            SKU = reader.GetString("SKU"),
                            FirstImageUrl = firstImageName != null ? GetImageUrl(reader.GetString("SKU"), firstImageName) : null
                        });
                    }
                }
            }
        }

        // Return the set of items along with the total count
        return (items, totalCount);
    }

    public string? GetImageUrl(string sku, string picName)
    {
        if (picName == null)
        {
            return null;
        }

        string firstImageKey = $"{sku}/{picName}";
        // Assume _s3Client and bucketName are configured elsewhere in the DataAccessLayer
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = firstImageKey,
                Expires = DateTime.Now.AddMinutes(60)
            };

            return _s3Client.GetPreSignedURL(request);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            return null;
        }
    }


    public async Task<int> CountItems()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var query = "SELECT COUNT(*) FROM CraftItem WHERE Listed = 1";
            using (var command = new MySqlCommand(query, connection))
            {
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
        }
    }
}