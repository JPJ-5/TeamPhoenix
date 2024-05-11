using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        public bool IsUserInterested(string username, int gigID)
        {
            string interestSql = "SELECT * FROM GigInterest WHERE GigID = @gigID, InterestedUser = @username;";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                // Update the InterestedUsers column in the database
                using (var updateCommand = new MySqlCommand(interestSql, connection))
                {
                    updateCommand.Parameters.AddWithValue("@username", username);
                    updateCommand.Parameters.AddWithValue("@gigID", gigID);
                    using (var reader = updateCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public bool IndicateInterest(string username, int gigID)
        {

            string updateSql = "INSERT INTO GigInterest (GigID, InterestedUser, GigPoster) VALUES (@gigID, @username, @poster);";
            string? poster = GetPosterUsername(gigID);
            if (poster == null) { return false; }
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                // Update the InterestedUsers column in the database
                using (var updateCommand = new MySqlCommand(updateSql, connection))
                {
                    updateCommand.Parameters.AddWithValue("@username", username);
                    updateCommand.Parameters.AddWithValue("@gigID", gigID);
                    updateCommand.Parameters.AddWithValue("@poster", poster);

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected == 1)
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        public string? GetPosterUsername(int gigID)
        {
            string posterSql = "SELECT PosterUsername FROM Gig WHERE GigID = @gigID;";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                // Update the InterestedUsers column in the database
                using (var updateCommand = new MySqlCommand(posterSql, connection))
                {
                    updateCommand.Parameters.AddWithValue("@gigID", gigID);
                    using (var reader = updateCommand.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            return reader["PosterUsername"].ToString() ?? string.Empty;
                        }
                    }
                }
            }
            return null;
        }
    }
}