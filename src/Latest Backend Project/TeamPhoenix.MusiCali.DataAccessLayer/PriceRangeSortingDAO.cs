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
    public async Task<(HashSet<Item> items, int totalCount)> FetchPagedItems(int pageNumber, int pageSize, string? name = null, decimal? bottomPrice = null, decimal? topPrice = null)
    {
        // Initialize an empty set to store items and an integer to store the total count
        var items = new HashSet<Item>();
        int totalCount = 0;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            // Open database connection asynchronously
            await connection.OpenAsync();
            var offset = (pageNumber - 1) * pageSize;

            // Create a base filter query to filter data
            var baseFilter = new StringBuilder("FROM CraftItem WHERE Listed = 1");

            var conditions = new List<string>();

            // Add search conditions based on parameters
            if (!string.IsNullOrWhiteSpace(name))
            {
                conditions.Add("LOWER(Name) LIKE CONCAT(@nameQuery, '%')");
            }
            if (bottomPrice.HasValue && topPrice.HasValue)
            {
                conditions.Add("Price BETWEEN @bottomPrice AND @topPrice");
            }

            // Append conditions to the base filter
            if (conditions.Count > 0)
            {
                baseFilter.Append(" AND ");
                baseFilter.Append(string.Join(" AND ", conditions));
            }

            // Create a count query to find the total count of filtered items
            var countQuery = $"SELECT COUNT(*) {baseFilter}";

            // Create a paginated query to retrieve the filtered items
            var dataQuery = new StringBuilder("SELECT Name, Price, SKU, Image ");
            dataQuery.Append(baseFilter);
            dataQuery.Append(" ORDER BY Price ASC LIMIT @pageSize OFFSET @offset");

            // Execute the count query to determine the total filtered count
            using (MySqlCommand countCommand = new MySqlCommand(countQuery, connection))
            {
                // Add parameters to the command based on filter criteria
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

                // Get the count result
                totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
            }

            // Execute the query to retrieve paginated data
            using (MySqlCommand dataCommand = new MySqlCommand(dataQuery.ToString(), connection))
            {
                // Add parameters for filtering items
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

                // Read results and add each item to the set
                using (var reader = await dataCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // Get first image name if present
                        var imageString = reader.GetString("Image");
                        var firstImageName = imageString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                        // Add item details to the collection
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

        // Return the items and the total count as a tuple
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