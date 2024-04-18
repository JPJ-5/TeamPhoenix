using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using static Mysqlx.Notice.Warning.Types;
using rU = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser;
//using _loggerCreation = TeamPhoenix.MusiCali.Logging.Logger;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class BingoBoard
    {
        public static GigSet? ViewGigSummary(int numberOfGigs, string currentUsername)
        {
            string level;
            string category;
            string context;
            string userHash;

            GigSet gigs = new();

            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            string viewGigSummarySql = "SELECT PosterUsername, GigName, GigDateTime, Location, Pay, Description FROM Gig WHERE GigVisibility = TRUE ORDER BY GigDateTime LIMIT @GigLoadLimit";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(viewGigSummarySql, connection))
                {
                    command.Parameters.AddWithValue("@GigLoadLimit", numberOfGigs);

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
                                reader["Description"].ToString() ?? string.Empty
                                );
                            gigs.GigSummaries.Add(newGig);
                            //reader.NextResult();
                        }
                        if (gigs.GigSummaries.Count == 0)
                        {
                            userHash = rU.GetUserHash(currentUsername);
                            level = "Info";
                            category = "View";
                            context = "Failed to retrieve gigs";
                            //_loggerCreation.CreateLog(userHash, level, category, context);
                            return null;
                        }
                        if (gigs.GigSummaries.Count == numberOfGigs)
                        {
                            userHash = rU.GetUserHash(currentUsername);
                            level = "Info";
                            category = "View";
                            context = $"{numberOfGigs} gigs successfully retrieved from database";
                            //_loggerCreation.CreateLog(userHash, level, category, context);
                        }
                        else
                        {
                            userHash = rU.GetUserHash(currentUsername);
                            level = "Info";
                            category = "View";
                            context = $"{gigs.GigSummaries.Count} gigs successfully retrieved from database, but {numberOfGigs} were requested";
                            //_loggerCreation.CreateLog(userHash, level, category, context);
                        }
                        return gigs;
                    }
                }
            }
        }

        public static int? ReturnNumOfGigs()
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

            return null;
        }
    }
}