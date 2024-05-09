//using Amazon.S3.Model;
//using Amazon.S3;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Configuration;
//using TeamPhoenix.MusiCali.DataAccessLayer;
//using TeamPhoenix.MusiCali.Logging;
//using TeamPhoenix.MusiCali.Security;
//using TeamPhoenix.MusiCali.DataAccessLayer.Models;

//namespace TeamPhoenix.MusiCali.Services
//{
//    public class PaginationService
//    {
//        //private readonly AmazonS3Client _s3Client;
//        private ItemCreationDAO itemCreationDAO;
//        private readonly IConfiguration configuration;
//        private LoggerService loggerService;
//        private AuthenticationSecurity authenticationSecurity;
//        private readonly IAmazonS3 _s3Client;
//        private readonly string bucketName;

//        public PaginationService(IAmazonS3 s3Client, IConfiguration configuration)
//        {
//            _s3Client = s3Client;
//            this.configuration = configuration;
//            itemCreationDAO = new ItemCreationDAO(this.configuration);
//            loggerService = new LoggerService(this.configuration);
//            authenticationSecurity = new AuthenticationSecurity(this.configuration);
//            bucketName = configuration.GetValue<string>("AWS:BucketName");
            
//        }

//        public string GetImageUrl(string key)
//        {
//            return _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
//            {
//                BucketName = bucketName,
//                Key = key,
//                Expires = DateTime.UtcNow.AddHours(1), // 1 hour validity
//                Verb = HttpVerb.GET
//            });
//        }

//        public List<string> GetImageUrls(List<string> keys)
//        {
//            return keys.Select(key => GetImageUrl(key)).ToList();
//        }

//        //public string? GetImageUrl(ItemCreationModel item, int picIndex) // getting s3 pic presigned url is synchonous task, no need await.
//        //{
//        //    if (item.ImageUrls == null || item.ImageUrls.Count == 0 || item.ImageUrls.Count() < picIndex + 1)
//        //    {
//        //        return null;
//        //    }

//        //    string firstImageKey = $"{item.Sku}/{item.ImageUrls[picIndex]}";

//        //    try
//        //    {
//        //        var request = new GetPreSignedUrlRequest
//        //        {
//        //            BucketName = bucketName,
//        //            Key = firstImageKey,
//        //            Expires = DateTime.Now.AddMinutes(60) // URL valid for 60 minutes
//        //        };

//        //        string url = _s3Client.GetPreSignedURL(request);
//        //        return url;
//        //    }
//        //    catch (AmazonS3Exception e)
//        //    {
//        //        Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
//        //        return null;
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
//        //        return null;
//        //    }
//        //}
//    }

//}
