using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class CollabSearchDataAccess
    {
        private readonly string _connectionString;

        public CollabSearchDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<DataTable> SearchUsersAsync(string username)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = "SELECT * FROM ArtistProfile WHERE Username = @username AND ArtistCollabSearchVisibility = false";
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@username", username);
                var reader = await command.ExecuteReaderAsync();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
        }


        public async Task UpdateUserVisibilityAsync(string username, string visibility)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = "UPDATE ArtistProfile SET ArtistCollabSearchVisibility = @visibility WHERE Username = @username";
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@visibility", visibility);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}