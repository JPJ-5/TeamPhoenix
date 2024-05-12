using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class ItemDeletionDAO
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;
        public ItemDeletionDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }

        public async Task<bool> DeleteItem(string userHash, string SKU)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userHash))
                {
                    throw new Exception("ItemDeletionError: ");
                }
                if (string.IsNullOrWhiteSpace(SKU))
                {
                    throw new Exception("UserDeletionError: Invalid SKU");
                }
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "DELETE FROM CraftItem WHERE CreatorHash = @CreatorHash AND SKU = @SKU";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CreatorHash", userHash);
                        command.Parameters.AddWithValue("@SKU", SKU);
                        var result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
