using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using rU = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser;
//using _loggerCreation = TeamPhoenix.MusiCali.Logging.Logger;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class BingoBoard
    {
        public static GigSet ViewGigSummary(int numberOfGigs, string currentUsername)
        {
            string level;
            string category;
            string context;
            string userHash;

            GigSet gigs = new();

            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            string viewGigSummarySql = "SELECT PosterUsername, GigName, GigDateTime, Location, Pay, Description FROM Gig WHERE GigVisibility = TRUE ORDER BY GigID LIMIT @GigLoadLimit";

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

        public static List<GigSummary>? ViewGigSummaryByPostID(string currentUsername, DateTime gigDateTime)
        {
            string level;
            string category;
            string context;
            string userHash;

            List<GigSummary> gigs = new List<GigSummary>();

            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            string viewGigSummarySql = "SELECT PosterUsername, GigName, GigDateTime FROM Gig WHERE GigVisibility = TRUE AND AND GigDateTime = @GigDateTime ORDER BY GigDateTime";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(viewGigSummarySql, connection))
                {
                    command.Parameters.AddWithValue("@GigDateTime", gigDateTime);

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
                            gigs.Add(newGig);
                            //reader.NextResult();
                        }
                        if (gigs.Count == 0)
                        {
                            userHash = rU.GetUserHash(currentUsername);
                            level = "Info";
                            category = "View";
                            context = "Failed to retrieve gigs.";
                            //_loggerCreation.CreateLog(userHash, level, category, context);
                            return null;
                        }

                        else
                        {
                            userHash = rU.GetUserHash(currentUsername);
                            level = "Info";
                            category = "View";
                            context = $"{gigs.Count} gigs successfully retrieved from database.";
                            //_loggerCreation.CreateLog(userHash, level, category, context);
                        }
                        return gigs;
                    }
                }
            }
        }
    }
}