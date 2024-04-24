using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class DataAccessLayer
{
    private readonly string connectionString;

    public DataAccessLayer(IConfiguration configuration)
    {
        this.connectionString = configuration.GetConnectionString("ConnectionString")!;
    }

    public async Task<HashSet<Item>> FetchItems(decimal topPrice, decimal bottomPrice)
    {
        var items = new HashSet<Item>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var query = @"SELECT Name, Price FROM CraftItemTest 
                          WHERE Price BETWEEN @bottomPrice AND @topPrice 
                          ORDER BY Price ASC";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@topPrice", topPrice);
                command.Parameters.AddWithValue("@bottomPrice", bottomPrice);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(new Item
                        {
                            Name = reader.GetString(0),
                            Price = reader.GetDecimal(1)
                        });
                    }
                }
            }
        }
        return items;
    }

    public async Task<HashSet<Item>> FetchItemsByName(string query)
    {
        var items = new HashSet<Item>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var sql = @"SELECT Name, Price FROM CraftItemTest
                        WHERE LOWER(Name) LIKE CONCAT(@query, '%')
                        ORDER BY Name ASC";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@query", query.ToLower() + '%');

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(new Item
                        {
                            Name = reader.GetString("Name"),
                            Price = reader.GetDecimal("Price")
                        });
                    }
                }
            }
        }
        return items;
    }

    public async Task<HashSet<Item>> GetAllItems()
    {
        var items = new HashSet<Item>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var query = "SELECT Name, Price FROM CraftItemTest ORDER BY Price ASC";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(new Item
                        {
                            Name = reader.GetString(0),
                            Price = reader.GetDecimal(1)
                        });
                    }
                }
            }
        }
        return items;
    }

    public async Task<List<Item>> FetchPagedItems(int pageNumber, int pageSize)
    {
        var items = new List<Item>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            // Ensure pageNumber is treated as 1-based and correctly translated to 0-based for SQL OFFSET
            var offset = (pageNumber - 1) * pageSize;
            var query = @"
            SELECT  Name, Price, SKU
            FROM CraftItemTest 
            ORDER BY Price ASC 
            LIMIT @pageSize OFFSET @offset";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset",offset);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(new Item
                        {
                            Name = reader.GetString("Name"),
                            Price = reader.GetDecimal("Price")
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
            var query = "SELECT COUNT(*) FROM CraftItemTest";
            using (var command = new MySqlCommand(query, connection))
            {
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
        }
    }
}