using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class BingoBoardDAO
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;
        
        public BingoBoardDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }
        public GigSet? ViewGigSummary(int numberOfGigs, string currentUsername, int offset)
        {
            GigSet gigs = new();
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
                                Convert.ToInt32(reader["GigID"]),
                                IsUserInterested(currentUsername, Convert.ToInt32(reader["GigID"]))
                                );
                            gigs.GigSummaries!.Add(newGig);
                            //reader.NextResult();
                        }
                        return gigs;
                    }
                }
            }
        }

        public int ReturnNumOfGigs()
        {
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

        public List<string>? UserInterestList(string username, int gigID)
        {
            string selectSql = "SELECT InterestedUsers FROM Gig WHERE GigID = @gigID;";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                // Retrieve the current list of interested users
                using (var selectCommand = new MySqlCommand(selectSql, connection))
                {
                    selectCommand.Parameters.AddWithValue("@gigID", gigID);
                    string interestedUsersJson = (string)selectCommand.ExecuteScalar();

                    // Parse the JSON string to a list of usernames
                    List<string>? interestedUsers = JsonConvert.DeserializeObject<List<string>>(interestedUsersJson);

                    //ensure list is both not null and has values before returning
                    return interestedUsers;

                }
            }
        }

        public bool IsUserInterested(string username, int gigId)
        {
            List<string>?  interestedUsers = UserInterestList(username, gigId);
            if (interestedUsers != null && interestedUsers.Count > 0)
            {
                return interestedUsers.Contains(username);
            }
            return false;
        }

        public bool IndicateInterest(string username, int gigID)
        {
            List<string>?  interestedUsers = UserInterestList(username, gigID);
            
            // Add the new username to the list
            if(interestedUsers == null) { return false; }
            interestedUsers.Add(username);

            // Convert the updated list back to JSON format
            string updatedInterestedUsersJson = JsonConvert.SerializeObject(interestedUsers);


            string updateSql = "UPDATE Gig SET InterestedUsers = @interestedUsers WHERE GigID = @gigID;";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                // Update the InterestedUsers column in the database
                using (var updateCommand = new MySqlCommand(updateSql, connection))
                {
                    updateCommand.Parameters.AddWithValue("@interestedUsers", updatedInterestedUsersJson);
                    updateCommand.Parameters.AddWithValue("@gigID", gigID);
                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected == 1)
                    {
                        return true;
                    }
                }

            }

            return false;
        }
    }
}