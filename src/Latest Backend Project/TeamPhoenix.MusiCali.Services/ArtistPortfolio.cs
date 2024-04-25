using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Renci.SshNet;
using Microsoft.Extensions.Configuration;

namespace TeamPhoenix.MusiCali.Services
{
    public class ArtistPortfolio
    {
        private readonly IConfiguration? config;
        private readonly ArtistPortfolioDao? artistPortfolioDao;
        
        public ArtistPortfolio(IConfiguration configuration)
        {
            config = configuration;
            artistPortfolioDao = new ArtistPortfolioDao(configuration);
        }
        public ArtistProfileViewModel LoadArtistProfile(string username)
        {
            try
            {
                var file = artistPortfolioDao.GetPortfolio(username);
                var fileInfo = file[0];
                var localFiles = DownloadFilesLocally(fileInfo);
                var genreList = file[1];
                var descList = file[2];
                var artistInfo = file[3];
                if (artistInfo == null || fileInfo == null)
                {
                    throw new Exception($"Unable to access profile data");
                }

                var responseData = new ArtistProfileViewModel
                {
                    Occupation = artistInfo[0],
                    Bio = artistInfo[1],
                    Location = artistInfo[2],
                    File0 = GetFileBase64(localFiles[0]),
                    File1 = GetFileBase64(localFiles[1]),
                    File2 = GetFileBase64(localFiles[2]),
                    File3 = GetFileBase64(localFiles[3]),
                    File4 = GetFileBase64(localFiles[4]),
                    File5 = GetFileBase64(localFiles[5]),
                    File0Ext = Path.GetExtension(localFiles[0]),
                    File1Ext = Path.GetExtension(localFiles[1]),
                    File2Ext = Path.GetExtension(localFiles[2]),
                    File3Ext = Path.GetExtension(localFiles[3]),
                    File4Ext = Path.GetExtension(localFiles[4]),
                    File5Ext = Path.GetExtension(localFiles[5]),
                    File1Genre = genreList[0],
                    File2Genre = genreList[1],
                    File3Genre = genreList[2],
                    File4Genre = genreList[3],
                    File5Genre = genreList[4],
                    File1Desc = descList[0],
                    File2Desc = descList[1],
                    File3Desc = descList[2],
                    File4Desc = descList[3],
                    File5Desc = descList[4],
                };
                foreach (string path in localFiles)
                {
                    if (System.IO.File.Exists(path) && path is not null)
                    {
                        System.IO.File.Delete(path);
                    }
                }

                return responseData;
            }
            catch (Exception ex)
            {
                throw new Exception($"error in loading service for artist portfolio {ex.Message}");
            }
        }

        private string GetFileBase64(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return Convert.ToBase64String(fileBytes);
                }
                else
                {
                    Console.WriteLine($"File '{filePath}' does not exist.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file '{filePath}': {ex.Message}");
                return null;
            }
        }

        public async Task<Result> UploadFile(string username, int slot, IFormFile file, string genre, string desc)
        {
            var sshUsername = config.GetSection("SSHLogin:sshUsername").Value!;
            var sshHostname = config.GetSection("SSHLogin:sshHostname").Value!;
            var remoteFilePath = config.GetSection("SSHLogin:remoteFilePath").Value!;
            var fileName = file.FileName;
            var localFilePath = Path.Combine(Path.GetTempPath(), fileName); // Save file to a temporary location

            try
            {
                string privateKeyFilePath = Environment.GetEnvironmentVariable("JULIE_KEY");
                if (privateKeyFilePath == null)
                {
                    throw new InvalidOperationException("JULIE_KEY environmental variable is not set.");
                } // access backend vm enviromental variable
                // Save the uploaded file to the temporary location
                using (var stream = new FileStream(localFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var privateKeyFile = new PrivateKeyFile(privateKeyFilePath);
                var privateKeyAuthMethod = new PrivateKeyAuthenticationMethod(sshUsername, privateKeyFile);
                var connectionInfo = new Renci.SshNet.ConnectionInfo(sshHostname, sshUsername, privateKeyAuthMethod);

                using (var sshClient = new SshClient(connectionInfo))
                {
                    sshClient.Connect();

                    using (var scpClient = new ScpClient(connectionInfo))
                    {
                        scpClient.Connect();

                        // Upload the file to the remote server
                        scpClient.Upload(new FileInfo(localFilePath), remoteFilePath + fileName);

                        // Save the file path to the database
                        artistPortfolioDao.SaveFilePath(username, slot, remoteFilePath + fileName, genre, desc);
                    }


                    sshClient.Disconnect();
                }

                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"Error uploading file: {ex.Message}");
                return new Result { Success = false, ErrorMessage = ex.Message };
            }
            finally
            {
                // Delete the temporary file after uploading
                if (File.Exists(localFilePath))
                {
                    File.Delete(localFilePath);
                }
            }
        }

        public Result DeleteFile(string username, int slot)
        {
            try
            {
                // Get the file path from the database
                string filePath = artistPortfolioDao.GetFilePath(username, slot);

                string privateKeyFilePath = Environment.GetEnvironmentVariable("JULIE_KEY"); // access backend vm enviromental variable
                var sshUsername = config.GetSection("SSHLogin:sshUsername").Value!;
                var sshHostname = config.GetSection("SSHLogin:sshHostname").Value!;
                var remoteFilePath = config.GetSection("SSHLogin:remoteFilePath").Value!;

                var privateKeyFile = new PrivateKeyFile(privateKeyFilePath);
                var privateKeyAuthMethod = new PrivateKeyAuthenticationMethod(sshUsername, privateKeyFile);
                var connectionInfo = new Renci.SshNet.ConnectionInfo(sshHostname, sshUsername, privateKeyAuthMethod);

                using (var sshClient = new SshClient(connectionInfo))
                {
                    sshClient.Connect();

                    // Create an SSH command to delete the file
                    var deleteCommand = $"rm {filePath}";

                    // Log the delete command for debugging
                    Console.WriteLine($"Executing command: {deleteCommand}");

                    using (var sshCommand = sshClient.CreateCommand(deleteCommand))
                    {
                        // Execute the command
                        var result = sshCommand.Execute();

                        // Log the result of the command execution
                        Console.WriteLine($"Command execution result: {result}");
                    }
                    sshClient.Disconnect();

                    // Update the file path in the database to null
                    artistPortfolioDao.DeleteFilePath(username, slot);

                }

                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"Error deleting file from server: {ex.Message}");
                return new Result { Success = false, ErrorMessage = ex.Message };
            }
        }

        public List<string> DownloadFilesLocally(List<string> filePaths)
        {
            var localFilePaths = new List<string>();

            try
            {
                string privateKeyFilePath = Environment.GetEnvironmentVariable("JULIE_KEY");
                if (privateKeyFilePath == null)
                {
                    throw new InvalidOperationException("JULIE_KEY environmental variable is not set.");
                }

                var sshUsername = config.GetSection("SSHLogin:sshUsername").Value!;
                var sshHostname = config.GetSection("SSHLogin:sshHostname").Value!;
                var remoteFilePath = config.GetSection("SSHLogin:remoteFilePath").Value!;

                var privateKeyFile = new PrivateKeyFile(privateKeyFilePath);
                var privateKeyAuthMethod = new PrivateKeyAuthenticationMethod(sshUsername, privateKeyFile);
                var connectionInfo = new Renci.SshNet.ConnectionInfo(sshHostname, sshUsername, privateKeyAuthMethod);

                using (var sshClient = new SshClient(connectionInfo))
                {
                    sshClient.Connect();

                    using (var sftpClient = new SftpClient(connectionInfo))
                    {
                        try
                        {
                            sftpClient.Connect();

                            foreach (var filePath in filePaths)
                            {
                                if (!string.IsNullOrEmpty(filePath))
                                {
                                    // Extract file name from the file path
                                    var fileName = Path.GetFileName(filePath);

                                    // Generate a local file path to save the downloaded file
                                    var localFilePath = Path.Combine(Path.GetTempPath(), fileName);

                                    // Download the file from the remote server
                                    using (var fileStream = File.Create(localFilePath))
                                    {
                                        sftpClient.DownloadFile(filePath, fileStream);
                                    }

                                    // Add the local file path to the list
                                    localFilePaths.Add(localFilePath);
                                }
                                else
                                {
                                    // Add null to maintain slot order
                                    localFilePaths.Add(string.Empty);
                                }
                            }
                        }
                        finally
                        {
                            // Disconnect the SFTP client even if an exception occurs
                            sftpClient.Disconnect();
                        }
                    }

                    sshClient.Disconnect();
                }

                return localFilePaths;
            }
            catch (Renci.SshNet.Common.SshConnectionException sshEx)
            {
                throw new Exception($"SSH connection error: {sshEx.Message}", sshEx);
            }
            catch (Renci.SshNet.Common.SshAuthenticationException authEx)
            {
                throw new Exception($"SSH authentication error: {authEx.Message}", authEx);
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                throw new Exception($"Error downloading files locally: {ex.Message}", ex);
            }
        }


        public void DeleteLocalFiles(List<string> filePaths)
        {
            try
            {
                foreach (var filePath in filePaths)
                {
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        // Check if the file exists locally
                        if (File.Exists(filePath))
                        {
                            // Delete the file
                            File.Delete(filePath);
                        }
                        else
                        {
                            // Log a message indicating that the file does not exist locally
                            Console.WriteLine($"File does not exist at path: {filePath}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during file deletion
                throw new Exception($"Error deleting local files: {ex.Message}");
            }
        }

    }
}
