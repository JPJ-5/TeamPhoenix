using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using static Mysqlx.Notice.Warning.Types;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class BingoBoard
    {
        public static GigSet? ViewGigSummary(int numberOfGigs, string currentUsername, int offset)
        {
            GigSet gigs = new();

            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            string viewGigSummarySql = "SELECT PosterUsername, GigName, GigDateTime, Location, Pay, Description, GigID FROM Gig WHERE GigVisibility = TRUE ORDER BY GigDateTime LIMIT @GigLoadLimit OFFSET @pageOffset";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(viewGigSummarySql, connection))
                {
                    command.Parameters.AddWithValue("@GigLoadLimit", numberOfGigs);
                    command.Parameters.AddWithValue("@pageOffset", offset);

                    using (var reader = command.ExecuteReader())
                    {
                        //should add every gig summary to a gigsummary list to be returned
                        while (reader.Read())
                        {
                            var newGig = new GigSummary(
                                reader["PosterUsername"].ToString() ?? string.Empty,
                                reader["GigName"].ToString() ?? string.Empty,
                                Convert.ToDateTime(reader["GigDateTime"]),
                                reader["Location"].ToString() ?? string.Empty,
                                reader["Pay"].ToString() ?? string.Empty,
                                reader["Description"].ToString() ?? string.Empty,
                                Convert.ToInt32(reader["GigID"])
                                );
                            gigs.GigSummaries!.Add(newGig);
                            //reader.NextResult();
                        }
                        return gigs;
                    }
                }
            }
        }

        public static int ReturnNumOfGigs()
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            string gigTotalSql = "SELECT COUNT(GigId) AS NumberOfGigs FROM Gig WHERE GigVisibility = TRUE;";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(gigTotalSql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        //should return int number of visible gigs in database
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["NumberOfGigs"]);
                        }
                    }
                }
            }
            return 0;
        }

        public static bool IsUserInterested(string username, int gigID)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            string usernameInterestCheckSql = "SELECT COUNT(username) AS NumberOfGigs FROM Gig WHERE GigVisibility = TRUE;";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(usernameInterestCheckSql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        //should return int number of visible gigs in database
                        if (reader.Read())
                        {
                            //
                        }
                    }
                }
            }
            return false;
        }

        public static bool IndicateInterest(string username, int gigID)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            string usernameInterestAddSql = "UPDATE Gig SET InterestedUsers='{username}' WHERE GigID='{gigID}';";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(usernameInterestAddSql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@gigID", gigID);
                    var result = command.ExecuteNonQuery();
                    if(result == 1) { return true; }
                }
            }
            return false;
        }
    }
}