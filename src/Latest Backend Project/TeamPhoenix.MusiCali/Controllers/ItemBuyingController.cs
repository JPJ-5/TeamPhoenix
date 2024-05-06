using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
using System.Diagnostics.CodeAnalysis;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemBuyingController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ItemBuyingDAO itemBuyingDAO;
        private RecoverUserDAO recoverUserDAO;
        private ItemCreationDAO itemCreationDAO;
        private readonly CraftReceiptService craftReceiptService;
        public ItemBuyingController(IAmazonS3 s3Client, IConfiguration configuration)
        {
            this.configuration = configuration;
            itemBuyingDAO = new ItemBuyingDAO(this.configuration);
            recoverUserDAO = new RecoverUserDAO(configuration);
            itemCreationDAO = new ItemCreationDAO(this.configuration, s3Client);
            craftReceiptService = new CraftReceiptService (this.configuration, s3Client);
        }


        [HttpPost("CreateASaleReceipt")]
        public async Task<IActionResult> CreateAnSaleReceipt([FromBody] CraftReceiptModel receipt, [FromHeader] string username)
        {


            try
            {
                string buyerHash = recoverUserDAO.GetUserHash(username);    //remmember to check username existing
                //CraftReceiptService cR = new CraftReceiptService(configuration, s3Client);


                bool isSuccess = await craftReceiptService.CreateAReceiptAsync( buyerHash,  receipt.SKU!, receipt.Offerable, receipt.OfferPrice, receipt.Quantity);


                if (isSuccess)
                {
                    //HttpResponseMessage response = await client.GetAsync(uploadSandboxToS3ApiUrl);
                    return Ok(new { Message = "Sale Receipt created successfully", Sku = receipt.SKU }); // Changed from JsonResult to IActionResult with Ok result
                }
                return BadRequest(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(false);
            }
        }
    }
}
