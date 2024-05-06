using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadS3Controller : ControllerBase
    {
        //private readonly IConfiguration configuration;
        private readonly IAmazonS3 _s3Client;
        private readonly string? bucketName;
        private readonly string? uploadFolderPath;
        private ItemCreationDAO itemCreationDAO;
        private RecoverUserDAO recoverUserDAO;
        private ItemCreationService itemCreationService;

        public UploadS3Controller(IAmazonS3 s3Client, IConfiguration configuration)
        {
            //this.configuration = configuration;
            _s3Client = s3Client;
            bucketName = configuration.GetValue<string>("AWS:BucketName");
            uploadFolderPath = configuration.GetValue<string>("AWS:UploadFolderPath");
            itemCreationDAO = new ItemCreationDAO(configuration, s3Client);
            recoverUserDAO = new RecoverUserDAO(configuration);
            itemCreationService  = new ItemCreationService(s3Client,configuration);
        }



        //this api will upload all file from front end to the back end sandbox/username folder.
        [HttpPost("UploadFilesToSandBox")]
        public async Task<IActionResult> UploadFilesToSandBox([FromHeader] string username)
        {
           

            try
            {
                string userHashFolder = recoverUserDAO.GetUserHash(username);
                var files = Request.Form.Files;

                var basePath = Path.Combine(uploadFolderPath!, userHashFolder);


                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                foreach (var file in files)
                {
                    var filePath = Path.Combine(basePath, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }


                return Ok(new { Message = $"Files uploaded successfully to {basePath}." });


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // Log the error for debugging
                return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading file.");
            }
        }


        




        // this api will upload all file from a back end sandbox/username folder to a s3/sku folder
        //[HttpPost("UpLoadFolderToS3")]
        //public async Task<IActionResult> UploadFolderFiles([FromHeader]string username)
        //{

        //    string sku = itemCreationService.GenerateSku(12);
        //    while (itemCreationDAO.IsSkuDuplicate(sku))
        //    {
        //        sku = itemCreationService.GenerateSku(12);
        //    }

        //    // Construct the path to the user- and SKU-specific folder locally
        //    string userFolderPath = Path.Combine(uploadFolderPath!, username);
        //    DirectoryInfo dir = new DirectoryInfo(userFolderPath);

        //    if (!dir.Exists)
        //    {
        //        return BadRequest("Userhash folder not found.");
        //    }
        //    if (bucketName == null)
        //    {
        //        return BadRequest("S3 bucket name is invalid.");
        //    }

        //    var fileInfos = dir.GetFiles();
        //    var uploadUrls = new List<string>();

        //    foreach (var fileInfo in fileInfos)
        //    {
        //        // Construct the key to include only the SKU in the folder structure in S3
        //        string keyName = $"{sku}/{fileInfo.Name}";
        //        try
        //        {
        //            using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
        //            {
        //                var uploadRequest = new TransferUtilityUploadRequest
        //                {
        //                    InputStream = fileStream,
        //                    Key = keyName,
        //                    BucketName = bucketName,
        //                    // Optionally set the ACL if needed
        //                    //CannedACL = S3CannedACL.PublicRead
        //                };

        //                var fileTransferUtility = new TransferUtility(_s3Client);
        //                await fileTransferUtility.UploadAsync(uploadRequest);

        //                //var fileUrl = $"https://{bucketName}.s3.amazonaws.com/{keyName}";
        //                var fileUrl = $"{fileInfo.Name}::";
        //                uploadUrls.Add(fileUrl);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log the exception or handle it based on your error handling strategy
        //            return StatusCode(500, $"Error uploading file {fileInfo.Name}: {ex.Message}");
        //        }
        //    }

        //    return Ok(new { Message = "Files uploaded successfully", Urls = uploadUrls, Sku = sku });
        //}







        // this api will get all the files in a sku folder
        [HttpGet("GetAllFileInBucket")]
        public async Task<IActionResult> GetAllFilesAsync([FromBody]string sku)
        {
            if (string.IsNullOrEmpty(sku))
            {
                return BadRequest("SKU must be provided.");
            }

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");

            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = $"{sku.TrimEnd('/')}/"  // Ensure the path is correctly formatted
            };
            var result = await _s3Client.ListObjectsV2Async(request);

            var s3Objects = result.S3Objects.Select(s =>
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = s.Key,
                    Expires = DateTime.UtcNow.AddMinutes(60)  // URL expires in 60 min
                };
                return new S3ObjectModel()
                {
                    Name = s.Key,
                    PresignedUrl = _s3Client.GetPreSignedURL(urlRequest),
                };
            });

            return Ok(s3Objects);
        }

        //this api will return image file names and video file names
        [HttpGet("GetFileNameInBucket")]
        public async Task<IActionResult> GetFileNameAsync([FromHeader]string sku)
        {
            if (string.IsNullOrEmpty(sku))
            {
                return BadRequest("SKU must be provided.");
            }

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");

            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = $"{sku.TrimEnd('/')}/"  // Ensure the path is correctly formatted
            };
            var result = await _s3Client.ListObjectsV2Async(request);

            // Define file extension groups for images and videos
            var imageExtensions = new HashSet<string> { ".jpg", ".png", ".gif", ".tiff" };
            var videoExtensions = new HashSet<string> { ".mp4", ".mov" };

            var imageFiles = new List<string>();
            var videoFiles = new List<string>();

            foreach (var s3Object in result.S3Objects)
            {
                string extension = Path.GetExtension(s3Object.Key).ToLower();

                if (imageExtensions.Contains(extension))
                {
                    imageFiles.Add(s3Object.Key);
                }
                else if (videoExtensions.Contains(extension))
                {
                    videoFiles.Add(s3Object.Key);
                }
            }

            return Ok(new { Images = imageFiles, Videos = videoFiles });
        }




        //this api will upload a file into a sku folder
        [HttpPost("UploadAFile")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file, [FromBody]string sku)
        {
            if (string.IsNullOrEmpty(sku))
            {
                return BadRequest("SKU must be provided.");
            }

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");

            // Build the key using only the SKU and the filename
            string key = $"{sku.TrimEnd('/')}/{file.FileName}";

            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = key,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(request);
            return Ok($"File {key} uploaded to S3 successfully!");
        }


        
 
        // this api will get a preview presignedURL of a file in a sku folder
        [HttpGet("PreviewAFile")]
        public async Task<IActionResult> GetFileByKeyAsync([FromHeader] string sku,[FromHeader] string filename)
        {
            // Construct the S3 key using the SKU and filename
            string key = $"{sku}/{filename}";

            // Check if the specified bucket exists
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");

            try
            {
                // Attempt to retrieve the object from S3
                var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
                return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
            }
            catch (Amazon.S3.AmazonS3Exception ex)
            {
                // Handle cases where the object is not found or other S3 errors
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound($"File not found: {key}");

                // Return a general error
                return StatusCode(500, "Error retrieving file from S3: " + ex.Message);
            }
        }





        

        // this api will delete 1 file in the sku folder
        [HttpDelete("DeleteAFile")]
        public async Task<IActionResult> DeleteFileAsync([FromHeader] string sku,[FromBody] string filename)
        {
            if (string.IsNullOrEmpty(sku) || string.IsNullOrEmpty(filename))
            {
                return BadRequest("SKU and filename must be provided.");
            }

            // Construct the S3 key using the SKU and filename
            string key = $"{sku.TrimEnd('/')}/{filename}";

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist");

            // Attempt to delete the object from S3
            await _s3Client.DeleteObjectAsync(bucketName, key);
            return NoContent(); // HTTP 204, indicating success but no content returned
        }



        //this api will delete the whole sku folder by deleting each item first.
        [HttpDelete("DeleteSkuFolder")]
        public async Task<IActionResult> DeleteSkuFolderAsync([FromBody] string sku)
        {
            if (string.IsNullOrEmpty(sku))
            {
                return BadRequest("SKU must be provided.");
            }

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist");

            // List all objects in the SKU folder
            var listRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = $"{sku.TrimEnd('/')}/"  // Ensure the path is correctly formatted
            };

            var listResponse = await _s3Client.ListObjectsV2Async(listRequest);
            if (listResponse.S3Objects.Count == 0)
            {
                return NotFound("No files found in the specified SKU folder.");
            }

            // Delete all objects in the SKU folder
            var deleteRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName,
                Objects = listResponse.S3Objects.Select(obj => new KeyVersion { Key = obj.Key }).ToList()
            };

            try
            {
                var deleteResponse = await _s3Client.DeleteObjectsAsync(deleteRequest);
                return Ok($"Deleted {deleteResponse.DeletedObjects.Count} files from the SKU folder.");
            }
            catch (DeleteObjectsException doe)
            {
                // Handle potential errors that could arise during the delete operation
                return StatusCode(500, $"Error deleting files: {doe.Message}");
            }
        }



        



    }


}
