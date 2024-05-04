using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var query = "SELECT Name, Price FROM CraftItemTest WHERE Price BETWEEN @bottomPrice AND @topPrice ORDER BY Price ASC";
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
}

public class Item
{
    public string? Name { get; set; }
    public decimal Price { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is Item other)
        {
            return Name == other.Name && Price == other.Price;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Price);
    }
}
