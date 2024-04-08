using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.DataAccessLayer.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using _loggerCreation = TeamPhoenix.MusiCali.Logging.Logger;

namespace TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.DataAccessLayer
{
    public class BingoBoard
    {
        public static List<GigSummary>? ViewGigSummary(ushort numberOfGigs)
        {
            string level;
            string category;
            string context;
            string userHash;

            List<GigSummary> gigs = new List<GigSummary>();

            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            string viewGigSummarySql = "SELECT PosterUsername, GigName, GigDateTime FROM Gig WHERE GigVisibility = TRUE ORDER BY GigDateTime LIMIT @GigLoadLimit";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(viewGigSummarySql, connection))
                {
                    command.Parameters.AddWithValue("@GigLoadLimit", numberOfGigs);
                    
                    using(var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var newGig = new GigSummary(
                                reader["PosterUsername"].ToString() ?? string.Empty,
                                reader["GigName"].ToString() ?? string.Empty,
                                Convert.ToDateTime(reader["GigDateTime"])
                                );
                            gigs.Add(newGig);
                            //reader.NextResult();
                        }
                    }
                }
            }

                return null;
        }
    }
}
