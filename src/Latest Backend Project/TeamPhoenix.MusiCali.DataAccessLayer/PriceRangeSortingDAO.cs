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

    public async Task<HashSet<Item>> FetchPagedItems(int pageNumber, int pageSize, string? name = null, decimal? bottomPrice = null, decimal? topPrice = null)
    {
        var items = new HashSet<Item>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var offset = (pageNumber - 1) * pageSize;
            var baseQuery = new StringBuilder("SELECT Name, Price, SKU, Image FROM CraftItem WHERE Listed = 1");

            if (!string.IsNullOrWhiteSpace(name) || bottomPrice.HasValue || topPrice.HasValue)
            {
                var conditions = new HashSet<string>();

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
                    baseQuery.Append(" AND ");
                    baseQuery.Append(string.Join(" AND ", conditions));
                }
            }

            baseQuery.Append(" ORDER BY Price ASC LIMIT @pageSize OFFSET @offset");
            var query = baseQuery.ToString();

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    command.Parameters.AddWithValue("@nameQuery", name.ToLower());
                }
                if (bottomPrice.HasValue)
                {
                    command.Parameters.AddWithValue("@bottomPrice", bottomPrice.Value);
                }
                if (topPrice.HasValue)
                {
                    command.Parameters.AddWithValue("@topPrice", topPrice.Value);
                }
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", offset);

                using (var reader = await command.ExecuteReaderAsync())
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
        return items;
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
            var query = "SELECT COUNT(*) FROM CraftItem";
            using (var command = new MySqlCommand(query, connection))
            {
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
        }
    }
}