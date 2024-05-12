using MySql.Data.MySqlClient;
using System.Collections.Generic;
//using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class CollabFeatureDAL
    {
        private static readonly string _connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

        public static List<string> SearchUsers(string userSearch)
        {
            List<string> users = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT Username FROM ArtistProfile";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userSearch", userSearch);

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
            string ? email = null;

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
            return email!;
        }

        //Create function to save new collab to database
        public static Result InsertCollab(string senderUsername, string receiverUsername, string senderEmail, string receiverEmail)
        {
            Result result = new Result();

            // Check if collab already exists
            if (CollabExists(senderUsername, receiverUsername))
            {
                result.Success = false;
                result.ErrorMessage = "Collab already exists";
                return result;
            }

            // If collab doesn't exist, proceed with insertion
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
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
                            return result;
                        }
                    }
                }

                catch (Exception ex)
                {
                    // You can add more specific exception handling if needed
                    throw new Exception($"Error inserting collab: {ex.Message}");
                }
            }

            result.Success = true;
            return result;
        }

        //check if the collab already exists
        private static bool CollabExists(string senderUsername, string receiverUsername)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Collab WHERE SenderUsername = @senderUsername AND RecieverUsername = @receiverUsername";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@senderUsername", senderUsername);
                    command.Parameters.AddWithValue("@receiverUsername", receiverUsername);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
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

                string query = "SELECT DISTINCT RecieverUsername FROM Collab WHERE SenderUsername = @username AND Accepted = FALSE";

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
                
                string sql = "SELECT DISTINCT Senderusername FROM Collab WHERE RecieverUsername = @username AND Accepted = FALSE";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        {
                            while (reader.Read())
                            {
                                string senderUsername = reader.GetString(0);
                                receivedCollabs.Add(senderUsername);
                            }
                        }
                    }
                }
            }

            return receivedCollabs;
        }


        //for getting accepted collabs from logged- in user
        public static List<string> GetAcceptedCollabsByUsername(string username)
        {
            List<string> sentCollabs = new List<string>();
            List<string> receivedCollabs = new List<string>();

            List<string> accepted = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string sentQuery = "SELECT SenderUsername FROM Collab WHERE SenderUsername != @username AND Accepted = true";
                string receivedQuery = "SELECT DISTINCT RecieverEmail FROM Collab WHERE RecieverUsername != @username AND Accepted = true";

                sentCollabs = RetrieveCollabs(connection,sentQuery, username);
                receivedCollabs = RetrieveCollabs(connection, receivedQuery, username);

                accepted.AddRange(sentCollabs);
                accepted.AddRange(receivedCollabs);

            }

            return accepted;
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
    }
}