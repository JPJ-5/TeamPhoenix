using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;
using Amazon.S3.Model;
using Amazon.S3;

public class DataAccessLayer
{
    // Dependencies: database connection string and AWS S3 client
    private readonly string connectionString;
    private readonly IAmazonS3 _s3Client;
    private readonly string bucketName;

    // Constructor to initialize connection string and AWS S3 client
    public DataAccessLayer(IConfiguration configuration, IAmazonS3 s3Client)
    {
        this.connectionString = configuration.GetConnectionString("ConnectionString")!;
        bucketName = configuration.GetValue<string>("AWS:BucketName");
        _s3Client = s3Client;
    }

    // Method to fetch paginated and filtered items from the database
    public async Task<(HashSet<Item> items, int totalCount)> FetchPagedItems(ItemQueryParameters query)
    {
        var items = new HashSet<Item>();
        int totalCount = 0;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var offset = (query.PageNumber - 1) * query.PageSize;

            var baseFilter = new StringBuilder("FROM CraftItem WHERE Listed = 1");
            var conditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                conditions.Add("LOWER(Name) LIKE CONCAT(@nameQuery, '%')");
            }
            if (query.BottomPrice.HasValue && query.TopPrice.HasValue)
            {
                conditions.Add("Price BETWEEN @bottomPrice AND @topPrice");
            }

            if (conditions.Count > 0)
            {
                baseFilter.Append(" AND ");
                baseFilter.Append(string.Join(" AND ", conditions));
            }

            var countQuery = $"SELECT COUNT(*) {baseFilter}";
            var dataQuery = new StringBuilder("SELECT Name, Price, SKU, Image ");
            dataQuery.Append(baseFilter);
            dataQuery.Append(" ORDER BY Price ASC LIMIT @pageSize OFFSET @offset");

            using (MySqlCommand countCommand = new MySqlCommand(countQuery, connection))
            {
                if (!string.IsNullOrWhiteSpace(query.Name))
                {
                    countCommand.Parameters.AddWithValue("@nameQuery", query.Name.ToLower());
                }
                if (query.BottomPrice.HasValue)
                {
                    countCommand.Parameters.AddWithValue("@bottomPrice", query.BottomPrice.Value);
                }
                if (query.TopPrice.HasValue)
                {
                    countCommand.Parameters.AddWithValue("@topPrice", query.TopPrice.Value);
                }

                totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
            }

            using (MySqlCommand dataCommand = new MySqlCommand(dataQuery.ToString(), connection))
            {
                if (!string.IsNullOrWhiteSpace(query.Name))
                {
                    dataCommand.Parameters.AddWithValue("@nameQuery", query.Name.ToLower());
                }
                if (query.BottomPrice.HasValue)
                {
                    dataCommand.Parameters.AddWithValue("@bottomPrice", query.BottomPrice.Value);
                }
                if (query.TopPrice.HasValue)
                {
                    dataCommand.Parameters.AddWithValue("@topPrice", query.TopPrice.Value);
                }
                dataCommand.Parameters.AddWithValue("@pageSize", query.PageSize);
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

        return (items, totalCount);
    }

    // Method to generate a pre-signed URL for an item image stored in AWS S3
    public string? GetImageUrl(string sku, string picName)
    {
        if (picName == null)
        {
            return null;
        }

        string firstImageKey = $"{sku}/{picName}";

        try
        {
            // Create a request to generate a pre-signed URL with expiration
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = firstImageKey,
                Expires = DateTime.Now.AddMinutes(60)
            };

            // Return the pre-signed URL from AWS S3
            return _s3Client.GetPreSignedURL(request);
        }
        catch (Exception e)
        {
            // Log error to the console in case of an exception
            Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            return null;
        }
    }

    // Method to count the total items listed in the database
    public async Task<int> CountItems()
    {
        // Open a database connection asynchronously
        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            // Query to count the total items in the "CraftItem" table
            var query = "SELECT COUNT(*) FROM CraftItem WHERE Listed = 1";
            using (var command = new MySqlCommand(query, connection))
            {
                // Execute the query and return the count
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
        }
    }
}