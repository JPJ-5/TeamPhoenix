using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadS3Controller : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string? bucketName;
        private readonly string? uploadFolderPath;

        public UploadS3Controller(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            bucketName = configuration.GetValue<string>("AWS:BucketName");
            uploadFolderPath = configuration.GetValue<string>("AWS:UploadFolderPath");
        }

        [HttpPost("upload-folder")]
        public async Task<IActionResult> UploadFolderFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(uploadFolderPath!);
            if (!dir.Exists )
            {
                return BadRequest("Folder not found.");
            }
            if (bucketName == null)
            {
                return BadRequest("s3 bucket name is invalid.");
            }


            var fileInfos = dir.GetFiles();
            var uploadUrls = new List<string>();

            foreach (var fileInfo in fileInfos)
            {
                string keyName = $"uploads/{DateTime.UtcNow:yyyyMMddHHmmss}_{fileInfo.Name}";
                try
                {
                    using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                    {
                        var uploadRequest = new TransferUtilityUploadRequest
                        {
                            InputStream = fileStream,
                            Key = keyName,
                            BucketName = bucketName,
                            CannedACL = S3CannedACL.PublicRead
                        };

                        var fileTransferUtility = new TransferUtility(_s3Client);
                        await fileTransferUtility.UploadAsync(uploadRequest);

                        var fileUrl = $"https://{bucketName}.s3.amazonaws.com/{keyName}";
                        uploadUrls.Add(fileUrl);
                    }
                }
                catch (Exception ex)
                {
                    // Implement your error handling logic
                    return StatusCode(500, $"Error uploading file {fileInfo.Name}: {ex.Message}");
                }
            }

            return Ok(new { Message = "Files uploaded successfully", Urls = uploadUrls });
        }

        [HttpGet("GetAllFileInBucket")]
        public async Task<IActionResult> GetAllFilesAsync( string? prefix)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = prefix
            };
            var result = await _s3Client.ListObjectsV2Async(request);
            var s3Objects = result.S3Objects.Select(s =>
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = s.Key,
                    Expires = DateTime.UtcNow.AddMinutes(1)  //url expire in 1 min
                };
                return new S3ObjectModel()
                {
                    Name = s.Key.ToString(),
                    PresignedUrl = _s3Client.GetPreSignedURL(urlRequest),
                };
            });
            return Ok(s3Objects);
        }

        [HttpPost("UploadAFile")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file, string? prefix)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await _s3Client.PutObjectAsync(request);
            return Ok($"File {prefix}/{file.FileName} uploaded to S3 successfully!");
        }

        [HttpGet("PreviewAFile")]
        public async Task<IActionResult> GetFileByKeyAsync( string key)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
            var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
            return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
        }

        [HttpDelete("DeleteAFile")]
        public async Task<IActionResult> DeleteFileAsync( string key)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist");
            await _s3Client.DeleteObjectAsync(bucketName, key);
            return NoContent();
        }
    }


}
