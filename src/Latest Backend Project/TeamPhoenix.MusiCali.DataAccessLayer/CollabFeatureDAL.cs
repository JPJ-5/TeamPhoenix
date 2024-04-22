using MySql.Data.MySqlClient;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class CollabFeatureDAL
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
        //Create fuction to retrieve email from ArtistProfile using Username
        public static string GetEmailByUsername(string username)
        {
            string email = null;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT Email FROM UserAccount WHERE Username = @username";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            email = reader.GetString(0);
                        }
                    }
                }
            }
            return email;
        }

        //Create function to save new collab to database
        public static Result InsertCollab(string senderUsername, string receiverUsername, string senderEmail, string receiverEmail)
        {
            Result result = new Result();
            
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                try{
                    connection.Open();

                    string sql = "INSERT INTO Collab (SenderUsername, RecieverUsername, Accepted, SenderEmail, RecieverEmail) " +
                                "VALUES (@senderUsername, @receiverUsername, false, @senderEmail, @receiverEmail)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@senderUsername", senderUsername);
                        command.Parameters.AddWithValue("@receiverUsername", receiverUsername);
                        command.Parameters.AddWithValue("@senderEmail", senderEmail);
                        command.Parameters.AddWithValue("@receiverEmail", receiverEmail);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            result.Success = false;
                            result.ErrorMessage = "Failed to insert collab record";
                            //throw new Exception("Failed to insert collab record");
                            return result;
                        }
                    }
                }
                 
                catch (Exception ex)
                {
                    // You can add more specific exception handling if needed
                    throw new Exception($"Error updating UserProfile: {ex.Message}");
                }
            }

            result.Success = true;
            return result;
        }

        //Create second function to find collab based on sender and receiver users where accept = true now
        public static Result AcceptCollabByUsername(string senderUsername, string receiverUsername)
        {
            Result result = new Result();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "UPDATE Collab SET Accepted = true WHERE SenderUsername = @senderUsername AND RecieverUsername = @receiverUsername";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@senderUsername", senderUsername);
                    command.Parameters.AddWithValue("@receiverUsername", receiverUsername);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        result.Success = false;
                        result.ErrorMessage = "No collab found for the specified sender and receiver usernames.";
                    }
                    else
                    {
                        result.Success = true;
                        result.ErrorMessage = "Collab accepted successfully.";
                    }
                }
            }
            return result;
        }










        //creates function to get all collab tables from the currently logged- in user

        public static List<string> GetSentCollabsByUsername(string username)
        {
            List<string> sentCollabs = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Collab WHERE SenderUsername = @username";
                sentCollabs = RetrieveCollabs(connection, query, username);
            }

            return sentCollabs;
        }
        //function for getting received collabs from logged- in user
        public static List<string> GetReceivedCollabsByUsername(string username)
        {
            List<string> receivedCollabs = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Collab WHERE ReceiverUsername = @username";
                receivedCollabs = RetrieveCollabs(connection, query, username);
            }

            return receivedCollabs;
        }

        //for getting accepted collabs from logged- in user
        public static List<List<string>> GetAcceptedCollabsByUsername(string username)
        {
            List<List<string>> acceptedCollabs = new List<List<string>>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Collab WHERE (SenderUsername = @username OR ReceiverUsername = @username) AND Accepted = true";
                acceptedCollabs = RetrieveAcceptedCollabs(connection, query, username);
            }

            return acceptedCollabs;
        }

        private static List<string> RetrieveCollabs(MySqlConnection connection, string query, string username)
        {
            List<string> collabs = new List<string>();

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        collabs.Add(reader.GetString(0)); // Assuming the collab data is stored in the first column
                    }
                }
            }
            return collabs;
        }

        //helper function
        private static List<List<string>> RetrieveAcceptedCollabs(MySqlConnection connection, string query, string username)
        {
            List<List<string>> acceptedCollabs = new List<List<string>>();

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<string> collabData = new List<string>();
                        // Assuming the collab data is stored in the first and second columns
                        collabData.Add(reader.GetString(0));
                        collabData.Add(reader.GetString(1));
                        acceptedCollabs.Add(collabData);
                    }
                }
            }

            return acceptedCollabs;
        }
    }
}