using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using rU = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser; // used to get userHash
using TeamPhoenix.MusiCali.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using _loggerCreation = TeamPhoenix.MusiCali.Logging.Logger;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    //Add logging for failure instead of simple EX messages.
    public class ArtistCalendar
    {
        public static bool IsGigDateExist(string username, DateTime gigDateTime) //tracks if the user has a gig during that timeframe already.
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string checkDuplicateSql = "SELECT COUNT(*) FROM Gig WHERE PosterUsername = @PosterUsername AND GigDateTime = @GigDateTime";
                using (MySqlCommand cmd = new MySqlCommand(checkDuplicateSql, connection))
                {
                    cmd.Parameters.AddWithValue("@PosterUsername", username);
                    cmd.Parameters.AddWithValue("@GigDateTime", gigDateTime);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        public static bool CreateGig(string username, string gigName, DateTime dateOfGig, bool visibility, string description, string location, string pay)
        {
            //establish logging information to insert when logging is performed.
            string level;
            string category;
            string context;
            string userHash;
            Result sqlResult;
            try
            {
                var dao = new SqlDAO("julie", "j1234"); //getting access to the database
                string createGigSql = "INSERT INTO Gig (PosterUsername, GigName, GigDateTime, GigVisibility, Description, Location, Pay) VALUES (@PosterUsername, @GigName, @GigDateTime, @GigVisibility, @Description, @Location, @Pay)";

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                // Adding each parameter into a dictionary
                parameters.Add("@PosterUsername", username);
                parameters.Add("@GigName", gigName);
                parameters.Add("@GigDateTime", dateOfGig);
                parameters.Add("@GigVisibility", visibility);
                parameters.Add("@Description", description);
                parameters.Add("@Location", location);
                parameters.Add("@Pay", pay);
                sqlResult = dao.ExecuteSql(createGigSql, parameters);
            }
            catch (Exception ex)
            {
                level = "Error";
                category = "View";
                context = "Gig failed to be added";
                userHash = rU.GetUserHash(username);
                _loggerCreation.CreateLog(userHash, level, category, context);
                Console.WriteLine($"Error:{ex.ToString()}");
                return false;
            }
            //logging
            level = "Info";
            category = "View";
            context = "Gig was successfully added";
            userHash = rU.GetUserHash(username);
            _loggerCreation.CreateLog(userHash, level, category, context);

            Console.WriteLine("Data inserted successfully!");
            return sqlResult.Success;
        }
        public static bool DeleteGig(string username, DateTime dateOfGig)
        {
            //establish logging information to insert when logging is performed.
            string level;
            string category;
            string context;
            string userHash;
            Result sqlResult;
            try
            {
                var dao = new SqlDAO("julie", "j1234"); //getting access to the database
                string deleteGigSql = "DELETE FROM Gig WHERE PosterUsername = @GigUsername AND GigDateTime = @GigDateTime"; //modify details once given details about database.

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                // Adding each parameter into a dictionary
                parameters.Add("@GigUsername", username);
                parameters.Add("@GigDateTime", dateOfGig);
                sqlResult = dao.ExecuteSql(deleteGigSql, parameters);
            }
            catch (Exception ex)
            {
                level = "Info";
                category = "View";
                context = "Gig failed to be deleted";
                userHash = rU.GetUserHash(username);
                _loggerCreation.CreateLog(userHash, level, category, context);
                Console.WriteLine($"Error:{ex.ToString()}");
                return false;
            }
            level = "Info";
            category = "View";
            context = "User deleted gig on calendar";
            userHash = rU.GetUserHash(username);
            _loggerCreation.CreateLog(userHash, level, category, context);

            Console.WriteLine("Data deleted successfully!");
            return sqlResult.Success;
        }

        public static GigView? ViewGig(string username, string usernameOfOwner, DateTime dateOfGig)
        {
            //establish logging information to insert when logging is performed.
            string level;
            string category;
            string context;
            string userHash;

            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;"; //getting access to the database
            string viewGigSql = "SELECT PosterUsername, GigName, GigDateTime, GigVisibility, Description, Location, Pay FROM Gig WHERE PosterUsername = @GigUsername AND GigDateTime = @GigDateTime";
            //specific requirements on the read requires new code.
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(viewGigSql, connection))
                {
                    command.Parameters.AddWithValue("@GigUsername", usernameOfOwner);
                    command.Parameters.AddWithValue("@GigDateTime", dateOfGig);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["PosterUsername"].ToString() != username && (bool)reader["GigVisibility"] == false)
                            {
                                level = "Info";
                                category = "View";
                                context = "Failed to retrieve gig due to visibility";
                                userHash = rU.GetUserHash(usernameOfOwner);
                                _loggerCreation.CreateLog(userHash, level, category, context);
                                return null;
                            }
                            var gigToView = new GigView(
                                reader["PosterUsername"].ToString() ?? string.Empty,
                                reader["GigName"].ToString() ?? string.Empty,
                                Convert.ToDateTime(reader["GigDateTime"]),
                                (bool)reader["GigVisibility"],
                                reader["Description"].ToString() ?? string.Empty,
                                reader["Location"].ToString() ?? string.Empty,
                                reader["Pay"].ToString() ?? string.Empty
                            );
                            //logging
                            level = "Info";
                            category = "View";
                            context = "Viewing an existing gig";
                            userHash = rU.GetUserHash(usernameOfOwner);
                            _loggerCreation.CreateLog(userHash, level, category, context);
                            Console.WriteLine("Gig successfully viewed");

                            return gigToView;
                        }
                        else
                        {
                            level = "Info";
                            category = "View";
                            context = "Failed to retrieve gig data";
                            userHash = rU.GetUserHash(usernameOfOwner);
                            _loggerCreation.CreateLog(userHash, level, category, context);

                            return null;
                        }
                    }
                }
            }
        }

        public static bool EditGig(DateTime oldDateOfGig, string username, string gigName, DateTime newDateOfGig, bool visibility, string description, string location, string pay)
        {
            //establish logging information to insert when logging is performed.
            string level;
            string category;
            string context;
            string userHash;

            Result sqlResult;
            try
            {
                string editGigSql = "UPDATE Gig SET GigName = @GigName, GigDateTime = @GigDateTime, GigVisibility = @GigVisibility, Description = @Description, Location = @Location, Pay = @Pay WHERE PosterUsername = @GigUsername AND GigDateTime = @OldDateTime";
                var dao = new SqlDAO("julie", "j1234"); //getting access to the database
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                // Adding each parameter into a dictionary
                parameters.Add("@OldDateTime", oldDateOfGig);
                parameters.Add("@GigUsername", username);
                parameters.Add("@GigName", gigName);
                parameters.Add("@GigDateTime", newDateOfGig);
                parameters.Add("@GigVisibility", visibility);
                parameters.Add("@Description", description);
                parameters.Add("@Location", location);
                parameters.Add("@Pay", pay);
                sqlResult = dao.ExecuteSql(editGigSql, parameters);
            }
            catch (Exception ex)
            {
                //logging
                level = "Info";
                category = "View";
                context = "Unable to edit gig data";
                userHash = rU.GetUserHash(username);
                _loggerCreation.CreateLog(userHash, level, category, context);
                Console.WriteLine($"Error:{ex.ToString()}");
                return false;
            }

            //logging
            level = "Info";
            category = "View";
            context = "User edited gig on calendar";
            userHash = rU.GetUserHash(username);
            _loggerCreation.CreateLog(userHash, level, category, context);

            Console.WriteLine("Data edited successfully!");
            return sqlResult.Success;
        }
        public static bool ChangeGigVisibility(string username, bool visibility)
        {
            //establish logging information to insert when logging is performed.
            string level;
            string category;
            string context;
            string userHash;

            Result sqlResult;
            try
            {
                string editGigSql = "UPDATE Gig SET GigVisibility = @GigVisibility WHERE PosterUsername = @GigUsername";
                var dao = new SqlDAO("julie", "j1234"); //getting access to the database
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                // Adding each parameter into a dictionary
                parameters.Add("@GigUsername", username);
                parameters.Add("@GigVisibility", visibility);
                sqlResult = dao.ExecuteSql(editGigSql, parameters);
            }
            catch (Exception ex)
            {
                level = "Info";
                category = "View";
                context = "Privacy settings failed to update";
                userHash = rU.GetUserHash(username);
                _loggerCreation.CreateLog(userHash, level, category, context);
                Console.WriteLine($"Error:{ex.ToString()}");
                return false;
            }
            level = "Info";
            category = "View";
            context = "Artist visibility was successfully set";
            userHash = rU.GetUserHash(username);
            _loggerCreation.CreateLog(userHash, level, category, context);

            Console.WriteLine("Data edited successfully!");
            return sqlResult.Success;
        }
    }

}
