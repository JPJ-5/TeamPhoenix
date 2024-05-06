using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Renci.SshNet;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using TeamPhoenix.MusiCali.Logging;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace TeamPhoenix.MusiCali.Services
{
    public class ArtistPortfolio
    {
        private readonly IConfiguration config;
        private readonly ArtistPortfolioDao artistPortfolioDao;
        private LoggerService loggerService;

        public ArtistPortfolio(IConfiguration configuration)
        {
            config = configuration;
            artistPortfolioDao = new ArtistPortfolioDao(configuration);
            loggerService = new LoggerService(configuration);
        }
        public ArtistProfileViewModel LoadArtistProfile(string username)
        {
            try 
            {
                var file = artistPortfolioDao.GetPortfolio(username);
                if (file == new List<List<string>>())
                {
                    return new ArtistProfileViewModel();
                }
                var fileInfo = file[0];
                var localFiles = DownloadFilesLocally(fileInfo);
                if (localFiles == new List<string>())
                {
                    loggerService.LogError(username, "Error", "Data", "ArtistPortfolio, Error connecting to server and downloading files locally");
                    return new ArtistProfileViewModel();
                }
                var genreList = file[1];
                var descList = file[2];
                var artistInfo = file[3];
                if (artistInfo == null || fileInfo == null)
                {
                    return new ArtistProfileViewModel();
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
                    File1Name = Path.GetFileName(localFiles[1]),
                    File2Name = Path.GetFileName(localFiles[2]),
                    File3Name = Path.GetFileName(localFiles[3]),
                    File4Name = Path.GetFileName(localFiles[4]),
                    File5Name = Path.GetFileName(localFiles[5]),
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
            catch (Exception)
            {
                return new ArtistProfileViewModel();
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
                    return "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file '{filePath}': {ex.Message}");
                return "";
            }
        }

        public async Task<Result> UploadFile(string? username, int? slot, IFormFile file, string? genre, string? desc)
        {
            var sshUsername = config.GetSection("SSHLogin:sshUsername").Value!;
            var sshHostname = config.GetSection("SSHLogin:sshHostname").Value!;
            var remoteFilePath = config.GetSection("SSHLogin:remoteFilePath").Value!;
            var fileName = file.FileName;
            string saveFolder = @"C:\Users\Joshu\OneDrive\Documents\ClamCheck";

            // Construct the path for saving the scanned file
            string saveFilePath = Path.Combine(saveFolder, fileName);

            // Replace the line with the specified file path
            var localFilePath = saveFilePath;


            try
            {
                string? privateKeyFilePath = Environment.GetEnvironmentVariable("JULIE_KEY");
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
                        var res = artistPortfolioDao.SaveFilePath(username!, slot, remoteFilePath + fileName, genre!, desc!);
                        if (res.Success == false)
                        {
                            return new Result { Success = false, ErrorMessage = "unable to save filepath" };
                        }
                    }


                    sshClient.Disconnect();
                }

                return new Result { Success = true };
            }
            catch (Renci.SshNet.Common.SshConnectionException sshEx)
            {
                loggerService.LogError(username!, "Error", "Data", "ArtistPortfolio, Unable to connect to server and Upload file");
                return new Result { Success = false, ErrorMessage = sshEx.Message };
            }
            catch (Renci.SshNet.Common.SshAuthenticationException authEx)
            {
                loggerService.LogError(username!, "Error", "Data", "ArtistPortfolio, Unable to connect to server and Upload file");
                return new Result { Success = false, ErrorMessage = authEx.Message };
            }
            catch (Exception ex)
            {
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

        public Result DeleteFile(string? username, int? slot)
        {
            try
            {
                // Get the file path from the database
                string filePath = artistPortfolioDao.GetFilePath(username, slot);
                if (filePath == "")
                {
                    return new Result { Success = false, ErrorMessage = "error finding filepath to delete" };
                }

                string? privateKeyFilePath = Environment.GetEnvironmentVariable("JULIE_KEY"); // access backend vm enviromental variable
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
                    var res = artistPortfolioDao.DeleteFilePath(username, slot);
                    if (res.Success == false)
                    {
                        return new Result { Success = false, ErrorMessage = "error finding filepath to delete" };
                    }

                }

                return new Result { Success = true };
            }
            catch (Renci.SshNet.Common.SshConnectionException sshEx)
            {
                loggerService.LogError(username!, "Error", "Data", "ArtistPortfolio, Unable to connect to server and Delete file");
                return new Result { Success = false, ErrorMessage = sshEx.Message };
            }
            catch (Renci.SshNet.Common.SshAuthenticationException authEx)
            {
                loggerService.LogError(username!, "Error", "Data", "ArtistPortfolio, Unable to connect to server and Delete file");
                return new Result { Success = false, ErrorMessage = authEx.Message };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, ErrorMessage = ex.Message };
            }
        }

        public List<string> DownloadFilesLocally(List<string> filePaths)
        {
            var localFilePaths = new List<string>();

            try
            {
                string? privateKeyFilePath = Environment.GetEnvironmentVariable("JULIE_KEY");
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
            catch (Renci.SshNet.Common.SshConnectionException)
            {
                return new List<string>();
            }
            catch (Renci.SshNet.Common.SshAuthenticationException)
            {
                return new List<string>();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }
}
