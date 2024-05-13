using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MySqlX.XDevAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemCreationController : Controller
    {
        private readonly IConfiguration configuration;
        private ItemCreationDAO itemCreationDAO;
        private RecoverUserDAO recoverUserDAO;
        //private readonly ApiService _apiService;
        private ItemCreationService itemCreationService;
        private readonly string? uploadSandboxToS3ApiUrl;
        private readonly string? baseUrl;
        private readonly IAmazonS3 _s3Client;

        //public ItemCreationController(IConfiguration configuration, ApiService apiService)
        public ItemCreationController(IAmazonS3 s3Client, IConfiguration configuration)
        {
            this.configuration = configuration;
            itemCreationDAO = new ItemCreationDAO(this.configuration, s3Client);
            recoverUserDAO = new RecoverUserDAO(configuration);
            _s3Client = s3Client;
            itemCreationService = new ItemCreationService(s3Client,configuration);
            baseUrl = configuration.GetValue<string>("NestedAPI:BasedURL");
            uploadSandboxToS3ApiUrl = baseUrl + configuration.GetValue<string>("NestedAPI:SandboxToS3");
        } 


        [HttpPost("CreateAnItem")]
        //public IActionResult CreateAnItem([FromBody] ItemCreationModel item, [FromHeader] string username)
        public async Task<IActionResult> CreateAnItem([FromBody] ItemCreationModel item, [FromHeader] string username)
        {
            try
            {
                if (item.VideoUrls!.Count > 2 || item.ImageUrls!.Count > 5)
                {
                    return BadRequest(new { Message = "You can only include up to 5 images + 2 videos" });
                }
                ItemCreationService iC = new ItemCreationService(_s3Client, configuration);
                string creatorHash = recoverUserDAO.GetUserHash(username);
                string sku = itemCreationService.GenerateSku(12);
                while (itemCreationDAO.IsSkuDuplicate(sku))
                {
                    sku = itemCreationService.GenerateSku(12);
                }

                bool isSuccess = await iC.CreateAnItemAsync(item.Name, creatorHash, sku, item.Price, item.Description,
                item.StockAvailable, item.ProductionCost, item.OfferablePrice, item.SellerContact, item.Listed);

                //bool isSuccess = await iC.CreateAnItemAsync(item.Name, creatorHash, sku, item.Price, item.Description,
                //item.StockAvailable, item.ProductionCost, item.OfferablePrice, item.SellerContact);
                if (isSuccess)
                {
                    //HttpResponseMessage response = await client.GetAsync(uploadSandboxToS3ApiUrl);
                    return Ok(new { Message = "Item created successfully", Sku = sku }); // Changed from JsonResult to IActionResult with Ok result
                }
                return BadRequest(new { Message = "!isSuccess." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "in api catch");
                return BadRequest(new { Message = "An error occurred while creating the item.", Error = ex.Message });

            }
        }
    }
}
