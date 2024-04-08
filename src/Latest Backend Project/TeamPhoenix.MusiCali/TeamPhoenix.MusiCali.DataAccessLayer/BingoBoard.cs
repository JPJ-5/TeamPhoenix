using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.DataAccessLayer
{
    public class BingoBoard
    {
        public static Array? ViewGigSummary(ushort numberOfGigs)
        {
            string level;
            string category;
            string context;
            string userHash;

            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            string viewGigSummarySql = "SELECT PosterUsername, GigName, GigDateTime FROM Gig WHERE GigVisibility = TRUE ORDER BY GigDateTime LIMIT @giglimit";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(viewGigSummarySql, connection))
                {

                }
            }

                return null;
        }
    }
}
