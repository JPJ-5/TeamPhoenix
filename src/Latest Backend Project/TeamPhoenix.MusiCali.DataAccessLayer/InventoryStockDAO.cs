using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class InventoryStockDAO
{
    private readonly string connectionString;

    public InventoryStockDAO(IConfiguration configuration)
    {
        this.connectionString = configuration.GetConnectionString("ConnectionString")!;
    }
    public async Task<HashSet<InventoryStockModel>> GetStockList(string userHash)
    {
        try
        {
            var inventory = new HashSet<InventoryStockModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM CraftItem WHERE CreatorHash = @CreatorHash";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CreatorHash", userHash);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            inventory.Add(new InventoryStockModel
                            {
                                Name = reader["Name"].ToString(),
                                SKU = reader["SKU"].ToString(),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                StockAvailable = reader.GetInt32(reader.GetOrdinal("StockAvailable")),
                                DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated"))
                            });
                            
                        }
                        return inventory;
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
            throw;
        }
    }
}
