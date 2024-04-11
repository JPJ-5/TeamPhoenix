using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; //how we're connecting to the database using the connection string
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer // Replace YourNamespace with your actual namespace
{
    public class CollabSearchDao
    {
        private readonly string _connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;"

        public async Task<List<string>> SearchUsers(string userSearch) //userSearch means the user that's being typed into the search bar
        {
            List<string> users = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString)) 
            {
                await connection.OpenAsync(); //opens the connection string

                string sql = "SELECT Username FROM ArtistProfile WHERE Username LIKE @userSearch + '%' AND ArtistCollabSearchVisibility = 1"; //sets the string up for search

                using (SqlCommand command = new SqlCommand(sql, connection)) //let's us run the command
                {
                    command.Parameters.AddWithValue("@userSearch", userSearch);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string username = reader.GetString(0);
                            users.Add(username); 
                        }
                    }
                }
            }

            return users;
        }
    }
}
