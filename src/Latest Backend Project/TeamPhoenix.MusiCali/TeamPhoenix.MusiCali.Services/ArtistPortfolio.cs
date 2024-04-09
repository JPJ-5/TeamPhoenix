using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Renci.SshNet;
using Azure.Identity;

namespace TeamPhoenix.MusiCali.Services
{
    public class ArtistPortfolio
    {
        public static async Task<Result> UploadFile(string username, int slot, IFormFile file, string genre, string desc)
        {
            var privateKeyFilePath = "C:/Users/Joshu/Downloads/juliek.pem";
            var sshUsername = "ec2-user";
            var sshHostname = "3.142.241.151";
            var remoteFilePath = "/home/ubuntu/MusiCali/";
            var fileName = file.FileName;
            var localFilePath = Path.Combine(Path.GetTempPath(), fileName); // Save file to a temporary location

            try
            {
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
                        ArtistPortfolioDao.SaveFilePath(username, slot, remoteFilePath + fileName, genre, desc);
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

        public static Result DeleteFile(string username, int slot)
        {
            try
            {
                // Get the file path from the database
                string filePath = ArtistPortfolioDao.GetFilePath(username, slot);

                var privateKeyFilePath = "C:/Users/Joshu/Downloads/juliek.pem";
                var sshUsername = "ec2-user";
                var sshHostname = "3.142.241.151";

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

                    // Update the file path in the database to null
                    ArtistPortfolioDao.DeleteFilePath(username, slot);

                    sshClient.Disconnect();
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

        public static List<string> DownloadFilesLocally(List<string> filePaths)
        {
            var localFilePaths = new List<string>();

            try
            {
                var privateKeyFilePath = "C:/Users/Joshu/Downloads/juliek.pem";
                var sshUsername = "ec2-user";
                var sshHostname = "3.142.241.151";
                var remoteFilePath = "/home/ubuntu/MusiCali/";

                var privateKeyFile = new PrivateKeyFile(privateKeyFilePath);
                var privateKeyAuthMethod = new PrivateKeyAuthenticationMethod(sshUsername, privateKeyFile);
                var connectionInfo = new Renci.SshNet.ConnectionInfo(sshHostname, sshUsername, privateKeyAuthMethod);

                using (var sshClient = new SshClient(connectionInfo))
                {
                    sshClient.Connect();

                    using (var sftpClient = new SftpClient(connectionInfo))
                    {
                        sftpClient.Connect();

                        foreach (var filePath in filePaths)
                        {
                            if (filePath != string.Empty)
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

                    sshClient.Disconnect();
                }

                return localFilePaths;
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                throw new Exception($"Error downloading files locally: {ex.Message}");
            }
        }

        public static void DeleteLocalFiles(List<string> filePaths)
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
 