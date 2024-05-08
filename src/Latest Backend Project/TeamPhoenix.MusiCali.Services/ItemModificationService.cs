using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;

namespace TeamPhoenix.MusiCali.Services
{
    public class ItemModificationService
    {
        private readonly IConfiguration configuration;
        private ItemModificationDAO itemModificationDAO;
        private RecoverUserDAO recoverUserDAO;
        private LoggerService loggerService;
        private readonly string uploadFolderPath;
        private ItemCreationService itemCreationService;
        private IAmazonS3 s3Client;

        public ItemModificationService(IAmazonS3 s3Client, IConfiguration configuration)
        {
            this.configuration = configuration;
            itemModificationDAO = new ItemModificationDAO(configuration);
            recoverUserDAO = new RecoverUserDAO(configuration);
            loggerService =  new LoggerService(configuration);
            uploadFolderPath = configuration.GetValue<string>("AWS:UploadFolderPath");
            itemCreationService = new ItemCreationService(s3Client, configuration);
            this.s3Client = s3Client;
        }

        public async Task<bool> updateItemRequest(string username, ItemCreationModel model)
        {
            try {
                var userHash = recoverUserDAO.GetUserHash(username);
                if(userHash == null)
                {
                    throw new Exception("Invalid User Hash!");
                }
                //check if sku belong to userhash
                //scan virus here
                //validate the video and image name, type, size, extension

                if(!(await itemModificationDAO.checkItemOwner(userHash, model.Sku!)))
                {
                    throw new Exception("No Item Found");
                }


                if(model.ImageUrls!.Contains("string") && model.VideoUrls!.Contains("string"))
                {
                    string userFolderPath = Path.Combine(uploadFolderPath, userHash);
                    DirectoryInfo dir = new DirectoryInfo(userFolderPath);

                    if (dir.Exists)
                    {
                        var deletePreviousFiles = DeleteAllFilesInFolderAsync(model.Sku!);
                        if (!(await deletePreviousFiles))
                        {
                            throw new Exception("Unable To Remove Old Files");
                        }
                        List<string> uploadedUrls = await itemCreationService.UploadFolderFilesAsync(userHash, model.Sku!);          // upload file from sandbox to s3, return the file names.
                        if (itemCreationService.DeleteSandboxFolder(userFolderPath))
                        {
                            Console.WriteLine("sucess delete the sandbox folder.");
                        }
                        else
                        {
                            throw new ArgumentException("fail to delete the sandbox.");
                        }
                        var imageExtensions = new HashSet<string> { ".jpg", ".png", ".gif", ".tiff" };
                        var videoExtensions = new HashSet<string> { ".mp4", ".mov" };

                        List<string> images = uploadedUrls.Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
                        List<string> videos = uploadedUrls.Where(f => videoExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
                        model.ImageUrls = images;
                        model.VideoUrls = videos;
                    }
                }
                //string userFolderPath = Path.Combine(uploadFolderPath, userHash);
                var validate = checkUpdateByte(model);
                if (!validate)
                {
                    throw new Exception("Invalid Update Info!");
                }
                var requestStatus = await itemModificationDAO.ModifyItemTable(userHash, model);
                if (!requestStatus)
                {
                    throw new Exception("Unable To Update Item");
                }
                var logSuccess = 
                    loggerService.CreateLog(userHash, "Info", "Business", 
                        $"ItemModification: {userHash} Sucessfully Modify {model.Sku}");
                if(logSuccess.HasError == true)
                {
                    throw new Exception("Successfully Update Item But Unable To Log");
                }
                return true;
            }
            catch (Exception ex)
            {
                Result loggingError;
                if (ex.Message == "Invalid User Hash!")
                {
                    loggingError = loggerService.CreateLog(null!, "Error", "Data", $"ItemModification: {ex.Message}");
                }
                else{
                    loggingError = loggerService.CreateLog(recoverUserDAO.GetUserHash(username), "Error", "Server", $"ItemModification: {ex.Message}");
                }
                return false;
            }
        }

        public bool checkUpdateByte(ItemCreationModel model)
        {
            // Check string lengths
            if (model.Name.Length > 250 || (model.CreatorHash != null && model.CreatorHash.Length > 255) ||
                (model.Sku != null && model.Sku.Length > 12) || model.SellerContact.Length > 300 ||
                (model.ImageUrls!.Any(url => url.Length > 500)) ||
                (model.VideoUrls!.Any(url => url.Length > 500)))
            {
                return false;
            }
            // Check decimal precision and scale
            if (model.Price < 0m || model.Price > 1000000m || Decimal.Round(model.Price, 2) != model.Price ||
                model.ProductionCost < 0m || model.ProductionCost > 1000000m || Decimal.Round(model.ProductionCost, 2) != model.ProductionCost)
            {
                return false;
            }
            // Check integers
            if (model.StockAvailable < 0)
            {
                return false;
            }
            // If all checks are passed
            return true;
        }

        public async Task<bool> DeleteAllFilesInFolderAsync(string sku)
        {
            if (string.IsNullOrEmpty(sku))
            {
                throw new ArgumentException("SKU must be provided.");
            }

            // Ensure the bucket exists
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(s3Client, uploadFolderPath);
            if (!bucketExists)
            {
                throw new Exception($"Bucket {uploadFolderPath} does not exist");
            }

            // Construct the prefix to list objects within the SKU folder
            string prefix = $"{sku.TrimEnd('/')}/";

            // List all objects with the specified prefix (SKU folder)
            var listRequest = new ListObjectsV2Request
            {
                BucketName = uploadFolderPath,
                Prefix = prefix
            };
            ListObjectsV2Response listResponse;

            try
            {
                do
                {
                    // Getting current batch of files
                    listResponse = await s3Client.ListObjectsV2Async(listRequest);
                    foreach (var obj in listResponse.S3Objects)
                    {
                        // Deleting each file found
                        await s3Client.DeleteObjectAsync(uploadFolderPath, obj.Key);
                    }
                    // Set continuation token to continue listing files
                    listRequest.ContinuationToken = listResponse.NextContinuationToken;
                } while (listResponse.IsTruncated); // Continue while there are more files to be listed

                return true;
            }
            catch (Exception ex)
            {
                loggerService.CreateLog(null, "Error", "Server", $"Failed to delete all files in folder {sku}: {ex.Message}");
                return false;
            }
        }
    }
}
