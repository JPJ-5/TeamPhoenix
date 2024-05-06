using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using TeamPhoenix.MusiCali.Security;
using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;



namespace TeamPhoenix.MusiCali.Services
{
    public class ItemCreationService
    {
        private ItemCreationDAO itemCreationDAO;
        private readonly IConfiguration configuration;
        //private Dictionary<string, List<byte[]>> allowedFormatSignatures;
        private LoggerService loggerService;
        private AuthenticationSecurity authenticationSecurity;
        private Hasher hasher;
        private static Random random = new Random();
        private readonly IAmazonS3 _s3Client;
        private readonly string uploadFolderPath;
        private readonly string bucketName;
        //private const string allowedSku = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        //private const string allowedCreatorHash = "";
        //private const string allowed = "";
        //private const string allowedPrice = "";
        //private const string allowedDesc = "";
        //private const string allowedCost = "";
        //private const string allowedSellerContact = "";


        public ItemCreationService(IAmazonS3 s3Client, IConfiguration configuration)
        {
            this.configuration = configuration;
            itemCreationDAO = new ItemCreationDAO(this.configuration, s3Client);
            loggerService = new LoggerService(this.configuration);
            authenticationSecurity = new AuthenticationSecurity(this.configuration);
            hasher = new Hasher();
            bucketName = configuration.GetValue<string>("AWS:BucketName");
            uploadFolderPath = configuration.GetValue<string>("AWS:UploadFolderPath");
            _s3Client = s3Client;
        }


        private bool IsNullString(string checkString)
        {
            return checkString.IsNullOrEmpty();
        }

        private bool IsValidLength(string checkString, int minLength, int maxLength)
        {
            int length = checkString.Length;
            return length >= minLength && length <= maxLength;
        }

        public bool IsValidDigit(string checkString, string allowedPattern)
        {
            return Regex.IsMatch(checkString, allowedPattern);
        }
        public bool IsValidPrice(decimal price)
        {
            // Convert the decimal to a string with a culture-invariant format, ensuring no unnecessary decimal places
            string decimalString = price.ToString("0.##", CultureInfo.InvariantCulture);

            // Regular expression to match exactly 5 digits or 5 digits with 2 after the decimal point
            string pattern = @"^\d{1,5}(\.\d{2})?$";

            // Returns true if the input matches the pattern, false otherwise
            return Regex.IsMatch(decimalString, pattern);
        }

        public string GenerateSku(int allowedSkuNum)  // allowedSkuNum = 12
        {
            string allowedSku = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new StringBuilder(allowedSkuNum);
            Random random = new Random();  // Initializing the Random object

            for (int i = 0; i < allowedSkuNum; i++)
            {
                int index = random.Next(allowedSku.Length);  // Get a random index
                result.Append(allowedSku[index]);  // Append the character at the random index
            }

            return result.ToString();  // Convert StringBuilder to string and return
        }


        public async Task<bool> CreateAnItemAsync(string name, string creatorHash, string sku, decimal price, string desc, int stock,
            decimal cost, bool offer, string sellerContact, bool listed)
        {

            if (IsNullString(name) || !IsValidLength(name, 5, 250) || !IsValidDigit(name, @"^[a-zA-Z0-9@. -]*$"))
            {
                return false;
                throw new ArgumentException("Invalid item name provided. Retry again or contact system administrator");
            }
            if (IsNullString(creatorHash) || !IsValidLength(creatorHash, 5, 64) || !IsValidDigit(creatorHash, @"^[a-zA-Z0-9@]"))
            {
                return false;
                throw new ArgumentException("Invalid Creator hash provided. Retry again or contact system administrator");
            }
            if (IsNullString(desc) || !IsValidLength(desc, 1, 3000) || !IsValidDigit(desc, @"^[a-zA-Z0-9@. -]*$"))
            {
                return false;
                throw new ArgumentException("Invalid item description provided. Retry again or contact system administrator");
            }
            if (!IsValidPrice(price))
            {
                return false;
                throw new ArgumentException("Invalid item price provided. Retry again or contact system administrator");
            }
            if (stock < 0 || stock > 1000)
            {
                return false;
                throw new ArgumentException("Invalid item stock provided. Retry again or contact system administrator");
            }
            if (!IsValidPrice(cost))
            {
                return false;
                throw new ArgumentException("Invalid item cost provided. Retry again or contact system administrator");
            }
            if (IsNullString(sellerContact) || !IsValidLength(sellerContact, 1, 300) || !IsValidDigit(sellerContact, @"^[a-zA-Z0-9@. -]*$"))
            {
                return false;
                throw new ArgumentException("Invalid seller Contact provided. Retry again or contact system administrator");
            }

            try
            {

                //scan virus here
                //validate the video and image name, type, size, extension

                List<string> uploadedUrls = await UploadFolderFilesAsync(creatorHash, sku);          // upload file from sandbox to s3, return the file names.

            string userFolderPath = Path.Combine(uploadFolderPath, creatorHash);

                if (DeleteSandboxFolder(userFolderPath))
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











                //offer = false;
                //List<string> pic = new List<string>();
                //pic.Add("apple.jpg");
                //List<string> video = new List<string>();
                //video.Add("bear.mp4");
                //bool listed = false;





            
                ItemCreationModel newItem = new ItemCreationModel(name, creatorHash, sku, price, desc, stock,
                cost, offer, sellerContact, images, videos, listed);

            
                if (!itemCreationDAO.InsertIntoItemTable(newItem))
                {
                    throw new Exception("Unable To insert to item table");
                }
                string level = "Info";
                string category = "View";
                string context = "Creating new item";
                loggerService.CreateLog(creatorHash, level, category, context);
            }
            catch (SqlException ex1)
            {
                Console.WriteLine($"Error creating new Item in sql exception: {ex1.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating new Item: {ex.Message}");
                return false;
            }


            return true;
        }



        public async Task<List<string>> UploadFolderFilesAsync(string userHash, string sku)
        {
            string userFolderPath = Path.Combine(uploadFolderPath, userHash);
            DirectoryInfo dir = new DirectoryInfo(userFolderPath);

            if (!dir.Exists)
            {
                dir.Create(); // Create the directory if it does not exist
                Console.WriteLine("User directory created.");
            }
            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentException("S3 bucket name is invalid.");
            }

            var fileInfos = dir.GetFiles();
            var uploadUrls = new List<string>();

            foreach (var fileInfo in fileInfos)
            {
                string keyName = $"{sku}/{fileInfo.Name}";
                try
                {
                    using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                    {
                        var uploadRequest = new TransferUtilityUploadRequest
                        {
                            InputStream = fileStream,
                            Key = keyName,
                            BucketName = bucketName,
                            // Optionally set the ACL if needed
                            //CannedACL = S3CannedACL.PublicRead
                        };

                        var fileTransferUtility = new TransferUtility(_s3Client);
                        await fileTransferUtility.UploadAsync(uploadRequest);

                        
                        uploadUrls.Add(fileInfo.Name);
                    }
                }
                catch (Exception ex)
                {
                    // Here you would decide how to handle exceptions
                    // For example, you could log them, rethrow them, or collect the details for later use
                    Console.WriteLine($"Error uploading file {fileInfo.Name}: {ex.Message}");
                    throw; // Optionally rethrow
                }
            }

            return uploadUrls;
        }

        public bool DeleteSandboxFolder(string path)
        {
            try
            {
                // Check if the directory exists
                if (Directory.Exists(path))
                {
                    // Delete all files from the directory
                    foreach (var file in Directory.GetFiles(path))
                    {
                        File.Delete(file); // Delete each file
                    }
                    // Delete the directory after the files have been deleted
                    Directory.Delete(path);
                    Console.WriteLine("Sandbox directory deleted successfully.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Directory does not exist or has already been deleted.");
                    return false;
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"An I/O error occurred: {ioEx.Message}");
                return false;
            }
            catch (UnauthorizedAccessException uaEx)
            {
                Console.WriteLine($"Access denied: {uaEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public string? GetImageUrl(ItemCreationModel item, int picIndex) // getting s3 pic presigned url is synchonous task, no need await.
        {
            if (item.ImageUrls == null || item.ImageUrls.Count == 0 || item.ImageUrls.Count() < picIndex + 1)
            {
                return null;
            }

            string firstImageKey = $"{item.Sku}/{item.ImageUrls[picIndex]}";

            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = firstImageKey,
                    Expires = DateTime.Now.AddMinutes(60) // URL valid for 60 minutes
                };

                string url = _s3Client.GetPreSignedURL(request);
                return url;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                return null;
            }
        }





    }
}

