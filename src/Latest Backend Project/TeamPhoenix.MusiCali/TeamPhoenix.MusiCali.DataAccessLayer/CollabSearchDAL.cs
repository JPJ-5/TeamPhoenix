using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class CollabSearchDao
    {
        private static readonly string _connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

        public static List<string> SearchUsers(string userSearch)
        {
            string search = userSearch + '%';
            List<string> users = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT Username FROM ArtistProfile WHERE Username LIKE @userSearch AND ArtistCollabSearchVisibility = 1";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userSearch", search);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
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
