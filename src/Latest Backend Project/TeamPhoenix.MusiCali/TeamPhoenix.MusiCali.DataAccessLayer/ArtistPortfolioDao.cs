using System;
using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Collections.Generic;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class ArtistPortfolioDao
    {
        public static string _connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

        public static Result SaveFilePath(string username, int slot, string filePath, string genre, string desc)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Update the file path in the database
                    var query = $"UPDATE ArtistProfile SET File{slot}Path = @FilePath";

                    // If 0 then it's for a picture and only needs file path
                    if (slot == 0)
                    {
                        query += " WHERE Username = @Username";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FilePath", filePath);
                            command.Parameters.AddWithValue("@Username", username);

                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        query += $", File{slot}Genre = @FileG, File{slot}Desc = @FileD WHERE Username = @Username";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FilePath", filePath);
                            command.Parameters.AddWithValue("@FileG", genre);
                            command.Parameters.AddWithValue("@FileD", desc);
                            command.Parameters.AddWithValue("@Username", username);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred adding file path to the database: {ex.Message}");
            }
        }



        public static Result DeleteFilePath(string username, int slot)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    if (slot == 0)
                    {
                        var query = $"UPDATE ArtistProfile SET File{slot}Path = NULL WHERE Username = @Username";
                        // Update the file path in the database
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Username", username);

                            command.ExecuteNonQuery();
                        }
                    }

                    else
                    {
                        // Update the file path in the database
                        var query = $"UPDATE ArtistProfile SET File{slot}Path = NULL, File{slot}Genre = NULL, File{slot}Desc = NULL WHERE Username = @Username";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Username", username);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred deleting file path from the database: {ex.Message}");
            }
        }

        public static string GetFilePath(string username, int slot)
        {
            try
            {
                string filePath = "";

                // SQL query to retrieve the file path based on the username and slot number
                string query = $"SELECT File{slot}Path FROM ArtistProfile WHERE Username = @Username";

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                filePath = reader[$"File{slot}Path"].ToString();
                            }
                        }
                    }
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred retrieving file path from server: {ex.Message}");
            }
        }

        public static string GetUsername(string username)
        {
            try
            {
                string artistUsername = "";

                string query = $"SELECT Username FROM ArtistProfile WHERE Username = @Username";

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                artistUsername = reader["Username"].ToString();
                            }
                        }
                    }
                }

                return artistUsername;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred retrieving username: {ex.Message}");
            }
        }

        public static List<string> GetProfileInfo(string username)
        {
            try
            {
                string occ = "";
                string bio = "";
                string loc = "";
                List<string> artistInfo = new List<string>();
                string query = $"SELECT ArtistOccupation, ArtistBio, ArtistLocation FROM ArtistProfile WHERE Username = @Username";

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                occ = reader[$"ArtistOccupation"].ToString() ?? string.Empty;
                                bio = reader[$"ArtistBio"].ToString() ?? string.Empty;
                                loc = reader[$"ArtistLocation"].ToString() ?? string.Empty;
                                artistInfo.Add(occ);
                                artistInfo.Add(bio);
                                artistInfo.Add(loc);
                            }
                            else
                            {
                                artistInfo.Add(string.Empty);
                                artistInfo.Add(string.Empty);
                                artistInfo.Add(string.Empty);
                            }
                        }
                    }
                }

                return artistInfo;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred retrieving genre: {ex.Message}");
            }
        }

        public static List<List<string>> GetAllFileInfo(string username)
        {
            try
            {
                List<string> filePaths = new List<string>();
                List<string> fileGenres = new List<string>();
                List<string> fileDescriptions = new List<string>();
                List<List<string>> fileData = new List<List<string>>();
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    for (int i = 0; i < 6; i++)
                    {
                        if (i == 0)
                        {
                            string query = $"SELECT File{i}Path FROM ArtistProfile WHERE Username = @Username";
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@Username", username);

                                using (MySqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        string filePath = reader[$"File{i}Path"].ToString() ?? string.Empty;
                                        filePaths.Add(filePath);
                                    }
                                    else
                                    {
                                        filePaths.Add(string.Empty); // If no record is found, add empty string to maintain slot order
                                    }
                                }

                            }
                        }
                        else
                        {
                            string query = $"SELECT File{i}Path, File{i}Genre, File{i}Desc FROM ArtistProfile WHERE Username = @Username";

                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@Username", username);

                                using (MySqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        string filePath = reader[$"File{i}Path"].ToString() ?? string.Empty;
                                        string fileGenre = reader[$"File{i}Genre"].ToString() ?? string.Empty;
                                        string fileDesc = reader[$"File{i}Desc"].ToString() ?? string.Empty;
                                        filePaths.Add(filePath);
                                        fileGenres.Add(fileGenre);
                                        fileDescriptions.Add(fileDesc);
                                    }
                                    else
                                    {
                                        filePaths.Add(string.Empty); // If no record is found, add empty string to maintain slot order
                                        fileGenres.Add(string.Empty);
                                        fileDescriptions.Add(string.Empty);
                                    }
                                }
                            }
                        }

                    }
                }
                fileData.Add(filePaths);
                fileData.Add(fileGenres);
                fileData.Add(fileDescriptions);
                return fileData;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred retrieving file paths: {ex.Message}");
            }
        }

    }
}
