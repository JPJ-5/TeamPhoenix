using System;
using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class ArtistPortfolioDao
    {
        private readonly IConfiguration? configuration;
        private readonly string? connectionString;

        public ArtistPortfolioDao(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetSection("ConnectionStrings:ConnectionString").Value!;
        }

        public Result SaveFilePath(string username, int? slot, string filePath, string genre, string desc)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
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
                return new Result() { Success = false, ErrorMessage = $"An error occurred adding file path to the database: {ex.Message}" };
            }
        }


        public Result DeleteFilePath(string? username, int? slot)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
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
                return new Result() { Success = false, ErrorMessage = $"An error occurred deleting file path from the database: {ex.Message}" };
            }
        }

        public string GetFilePath(string? username, int? slot)
        {
            try
            {
                string? filePath = "";

                // SQL query to retrieve the file path based on the username and slot number
                string query = $"SELECT File{slot}Path FROM ArtistProfile WHERE Username = @Username";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                filePath = reader[$"File{slot}Path"]!.ToString();

                            }
                        }
                    }
                }

                return filePath!;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public List<List<string>> GetPortfolio(string username)
        {
            try
            {
                List<string> filePaths = new List<string>();
                List<string> fileGenres = new List<string>();
                List<string> fileDescriptions = new List<string>();
                List<List<string>> ProfileData = new List<List<string>>();
                List<string> artistInfo = new List<string>();
                string occ = "";
                string bio = "";
                string loc = "";
                string query = $"SELECT ArtistOccupation, ArtistBio, ArtistLocation FROM ArtistProfile WHERE Username = @Username";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    for (int i = 0; i < 6; i++)
                    {
                        if (i == 0)
                        {
                            string query2 = $"SELECT File{i}Path FROM ArtistProfile WHERE Username = @Username";
                            using (MySqlCommand command = new MySqlCommand(query2, connection))
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
                            string query2 = $"SELECT File{i}Path, File{i}Genre, File{i}Desc FROM ArtistProfile WHERE Username = @Username";

                            using (MySqlCommand command = new MySqlCommand(query2, connection))
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

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

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
                ProfileData.Add(filePaths);
                ProfileData.Add(fileGenres);
                ProfileData.Add(fileDescriptions);
                ProfileData.Add(artistInfo);
                return ProfileData;
            }
            catch (Exception)
            {
                return new List<List<string>>();  
            }
        }

        public Result updateInfo(string username, string section, string info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string capSection = char.ToUpper(section[0]) + section.Substring(1);

                    // Update the file path in the database
                    var query = $"UPDATE ArtistProfile SET Artist{capSection} = @info WHERE Username = @Username";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@info", info);
                        command.Parameters.AddWithValue("@Username", username);

                        command.ExecuteNonQuery();
                    }
                }

                return new Result { Success = true };
            }
            catch (Exception ex)
            {

                throw new Exception($"An error occurred adding info to database: {ex.Message}");
            }
        }

        public Result DeleteSection(string username, string section)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string capSection = char.ToUpper(section[0]) + section.Substring(1);
                    var query = $"UPDATE ArtistProfile SET Artist{capSection} = NULL WHERE Username = @Username";
                    // Update the file path in the database
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        command.ExecuteNonQuery();
                    }
                }

                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                return new Result() { Success = false, ErrorMessage = $"An error occurred deleting section from the database: {ex.Message}" };
            }
        }

    }
}
        