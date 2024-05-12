using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer;
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


        




        



    }


}
