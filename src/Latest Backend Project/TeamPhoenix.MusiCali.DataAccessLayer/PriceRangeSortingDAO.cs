using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;

public class DataAccessLayer
{
    private readonly string connectionString;

    public DataAccessLayer(IConfiguration configuration)
    {
        this.connectionString = configuration.GetConnectionString("ConnectionString")!;
    }

    public async Task<HashSet<Item>> FetchPagedItems(int pageNumber, int pageSize, string? name = null, decimal? bottomPrice = null, decimal? topPrice = null)
    {
        var items = new HashSet<Item>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var offset = (pageNumber - 1) * pageSize;
            var baseQuery = new StringBuilder("SELECT Name, Price, SKU FROM CraftItem WHERE Listed = 1");

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
                        items.Add(new Item
                        {
                            Name = reader.GetString("Name"),
                            Price = reader.GetDecimal("Price"),
                            SKU = reader.GetString("SKU")
                        });
                    }
                }
            }
        }
        return items;
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