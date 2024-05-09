using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
     public class ItemCreationDAO
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;

        public ItemCreationDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }

        public bool CheckSkuDuplicate(string sku)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Check if the sku already exists in item table
                string checkDuplicateSql = "SELECT COUNT(*) FROM CraftItem WHERE SKU = @SKU";
                using (MySqlCommand cmd = new MySqlCommand(checkDuplicateSql, connection))
                {
                    cmd.Parameters.AddWithValue("@SKU", sku);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }

            }

        }


    }
}
