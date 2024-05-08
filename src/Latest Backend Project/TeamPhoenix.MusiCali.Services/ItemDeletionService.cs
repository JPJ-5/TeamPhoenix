using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using MySqlX.XDevAPI;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TeamPhoenix.MusiCali.Services
{
    public class ItemDeletionService
    {
        private readonly IConfiguration configuration;
        private ItemDeletionDAO itemDeletionDAO;
        private RecoverUserDAO recoverUserDAO;
        private LoggerService loggerService;
        private readonly IAmazonS3 s3Client;
        private readonly string? bucketName;

        public ItemDeletionService(IConfiguration configuration, IAmazonS3 s3Client)
        {
            this.configuration = configuration;
            this.s3Client = s3Client;
            bucketName = configuration.GetValue<string>("AWS:BucketName");
            itemDeletionDAO = new ItemDeletionDAO(configuration);
            recoverUserDAO = new RecoverUserDAO(configuration);
            loggerService = new LoggerService(configuration);
        }

        public async Task<bool> ItemDeletionRequest(string username, string Sku)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    throw new Exception("ItemDeletionService: EmptyOrWhiteSpace Username");
                }
                if (string.IsNullOrWhiteSpace(Sku))
                {
                    throw new Exception("ItemDeletionService: EmptyOrWhiteSpace SKU");
                }
                string userHash = recoverUserDAO.GetUserHash(username);
                if (string.IsNullOrWhiteSpace(userHash))
                {
                    throw new Exception($"Could not find user {username}");
                }

                bool deleteFolder = await DeleteSkuFolderAsync(Sku);
                if(!deleteFolder)
                {

                    throw new Exception();
                }
                bool deleteItemResult = await itemDeletionDAO.DeleteItem(userHash, Sku);
                if (deleteItemResult)
                {
                    Result loggedItemDeletion = loggerService.CreateLog(userHash, "Info", "View", $"User {userHash} Delete Item {Sku}");
                    return deleteItemResult;
                }
                return false;

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                string userHash = recoverUserDAO.GetUserHash(username);
                if (!string.IsNullOrWhiteSpace(userHash))
                {
                    Result loggedError = loggerService.CreateLog(userHash, "Error", "Business", $"User {userHash} Item Deletion Fail: {ex.Message}");
                    if (loggedError.Success == true)
                    {
                        return false;
                    }
                }
                Console.WriteLine("ItemDeletion: Invalid Username/UserHash/SKU. Unable To Log Error");
                return false;
            }
        }

        public async Task<bool> DeleteSkuFolderAsync(string sku)
        {
            if (string.IsNullOrEmpty(sku))
            {
                return false;  // SKU is necessary to continue
            }

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName);
            if (!bucketExists)
            {
                return false;  // Bucket must exist to delete a folder
            }

            // Prepare to list all objects within the specified "folder" (prefix)
            List<KeyVersion> keysToDelete = new List<KeyVersion>();
            string? continuationToken = null;
            do
            {
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = $"{sku.TrimEnd('/')}/",  // Format the prefix to match the virtual folder structure
                    ContinuationToken = continuationToken
                };

                var listResponse = await s3Client.ListObjectsV2Async(listRequest);
                if (listResponse.S3Objects.Count == 0 && continuationToken == null)
                {
                    return false;  // No files found to delete
                }

                keysToDelete.AddRange(listResponse.S3Objects.Select(obj => new KeyVersion { Key = obj.Key }));
                continuationToken = listResponse.NextContinuationToken;
            } while (continuationToken != null);

            // If no keys were found to delete, return false
            if (!keysToDelete.Any())
            {
                return false;
            }

            // Delete all objects in the SKU "folder"
            var deleteRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName,
                Objects = keysToDelete
            };

            try
            {
                var deleteResponse = await s3Client.DeleteObjectsAsync(deleteRequest);
                return deleteResponse.DeletedObjects.Count == keysToDelete.Count;  // Ensure all specified objects were deleted
            }
            catch (DeleteObjectsException)
            {
                return false;  // Handle exceptions by returning false, indicating failure
            }
        }
    }
}
