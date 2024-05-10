using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly string bucketName;

        public ItemModificationService(IAmazonS3 s3Client, IConfiguration configuration)
        {
            this.configuration = configuration;
            itemModificationDAO = new ItemModificationDAO(configuration);
            recoverUserDAO = new RecoverUserDAO(configuration);
            loggerService = new LoggerService(configuration);
            uploadFolderPath = configuration.GetValue<string>("AWS:UploadFolderPath");
            itemCreationService = new ItemCreationService(s3Client, configuration);
            this.s3Client = s3Client;
            bucketName = configuration.GetValue<string>("AWS:BucketName");
        }

        public async Task<bool> updateItemRequest(string username, ItemCreationModel model)
        {
            try
            {
                var userHash = recoverUserDAO.GetUserHash(username);
                if (userHash == null)
                {
                    throw new Exception("Invalid User Hash!");
                }
                bool validate = checkUpdateByte(model);
                if (!validate)
                {
                    throw new Exception("Invalid Update Info!");
                }
                //check if sku belong to userhash
                //scan virus here
                //validate the video and image name, type, size, extension

                if (!(await itemModificationDAO.checkItemOwner(userHash, model.Sku!)))
                {
                    throw new Exception("No Item Found");
                }

                var imageExtensions = new HashSet<string> { ".jpg", ".png", ".gif", ".tiff" };
                var videoExtensions = new HashSet<string> { ".mp4", ".mov" };

                

                string userFolderPath = Path.Combine(uploadFolderPath, userHash);  
                DirectoryInfo dir = new DirectoryInfo(userFolderPath);     // make a dir, create an empty folder in sandbox

                if ((!(dir.Exists && dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0)))   // if the folder is not empty, start to check and upload s3
                {
                    var deletePreviousFiles = await DeleteAllFilesUnderPrefixAsync(model.Sku!);        // delete the sku folder on aws s3
                    Console.WriteLine(deletePreviousFiles);
                    if (!(deletePreviousFiles))                         // cannot delete old s3 files
                    {
                        throw new Exception("Unable To Remove Old Files");
                    }
                    List<string> uploadedUrls = await itemCreationService.UploadFolderFilesAsync(userHash, model.Sku!);  // start upload new files from sandbox to s3, return the file names.
                    userFolderPath = Path.Combine(uploadFolderPath, userHash);     
                    if (itemCreationService.DeleteSandboxFolder(userFolderPath))             // delete the sandbox folder after s3 upload
                    {
                        Console.WriteLine("sucess delete the sandbox folder.");
                    }
                    else
                    {
                        throw new ArgumentException("fail to delete the sandbox.");
                    }
                    List<string> images = uploadedUrls.Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
                    List<string> videos = uploadedUrls.Where(f => videoExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
                    model.ImageUrls = images;
                    model.VideoUrls = videos;
                }
                else
                {
                    List<string> uploadedUrls = GetFilenamesInS3BucketPath(userHash, model.Sku!);
                    List<string> images = uploadedUrls.Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
                    List<string> videos = uploadedUrls.Where(f => videoExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
                    model.ImageUrls = images;
                    model.VideoUrls = videos;

                }



                //string userFolderPath = Path.Combine(uploadFolderPath, userHash);
                
                var requestStatus = await itemModificationDAO.ModifyItemTable(userHash, model);   //modify table
                if (!requestStatus)
                {
                    throw new Exception("Unable To Update Item");
                }
                var logSuccess =
                    loggerService.CreateLog(userHash, "Info", "Business",
                        $"ItemModification: {userHash} Sucessfully Modify {model.Sku}");
                if (logSuccess.HasError == true)
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
                else
                {
                    loggingError = loggerService.CreateLog(recoverUserDAO.GetUserHash(username), "Error", "Server", $"ItemModification: {ex.Message}");
                }
                return false;
            }
        }

        public bool checkUpdateByte(ItemCreationModel model)
        {
            // Check string lengths
            if (model.Name.Length > 250 || (model.CreatorHash != null && model.CreatorHash.Length > 64) ||
                (model.Sku != null && model.Sku.Length > 12) || model.SellerContact.Length > 300 ||
                (model.ImageUrls != null && model.ImageUrls.Any(url => url != null && url.Length > 500)) ||
                (model.VideoUrls != null && model.VideoUrls.Any(url => url != null && url.Length > 500)))
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

        


        public async Task<bool> DeleteAllFilesUnderPrefixAsync(string sku)
        {
            try
            {
                string prefix = $"{sku}/";

                // List all objects with the specified prefix
                ListObjectsV2Request listRequest = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = prefix
                };

                ListObjectsV2Response listResponse;
                var deleteObjectsRequest = new DeleteObjectsRequest { BucketName = bucketName };

                do
                {
                    listResponse = await s3Client.ListObjectsV2Async(listRequest);

                    foreach (var s3Object in listResponse.S3Objects)
                    {
                        deleteObjectsRequest.Objects.Add(new KeyVersion { Key = s3Object.Key });
                    }

                    listRequest.ContinuationToken = listResponse.NextContinuationToken;

                } while (listResponse.IsTruncated);

                if (deleteObjectsRequest.Objects.Count > 0)
                {
                    // Delete all objects under the specified prefix
                    var deleteResponse = await s3Client.DeleteObjectsAsync(deleteObjectsRequest);
                    Console.WriteLine($"Deleted {deleteResponse.DeletedObjects.Count} objects under prefix: {prefix}");
                    return true; // Successful deletion
                }
                else
                {
                    Console.WriteLine($"No objects found under prefix: {prefix}");
                    return false; // No files to delete
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error encountered on server: {e.Message}");
                return false; // Error while deleting
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown error encountered: {e.Message}");
                return false; // Error while deleting
            }
        }



        public List<string> GetFilenamesInS3BucketPath(string bucketName, string sku)
        {
            List<string> filenames = new List<string>();
            try
            {
               

                // Create the request to list objects
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = sku
                };

                ListObjectsV2Response response;

                do
                {
                    // Fetch the list of objects
                    response =  s3Client.ListObjectsV2Async(request).Result;

                    // Extract filenames (keys) from the response
                    foreach (S3Object entry in response.S3Objects)
                    {
                        filenames.Add(entry.Key);
                    }

                    // Update the continuation token for the next request
                    request.ContinuationToken = response.NextContinuationToken;

                } while (response.IsTruncated);

            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Amazon S3 error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }

            return filenames;
        }
    }
}
