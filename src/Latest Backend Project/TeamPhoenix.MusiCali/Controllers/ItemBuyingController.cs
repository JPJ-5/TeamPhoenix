using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1.Ocsp;
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
            itemBuyingDAO = new ItemBuyingDAO(this.configuration, s3Client);
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


                bool isSuccess = await craftReceiptService.CreateAReceiptAsync( buyerHash,  receipt.SKU!, receipt.PendingSale, receipt.OfferPrice, receipt.Quantity);


                if (isSuccess)
                {
                    //HttpResponseMessage response = await client.GetAsync(uploadSandboxToS3ApiUrl);
                    return Ok(new { Message = "Sale Receipt created successfully", Sku = receipt.SKU }); // Changed from JsonResult to IActionResult with Ok result
                }
                return BadRequest(new { Message = "Failed to create sale receipt" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(new { Message = "An unexpected error occurred: " + ex.Message });
            }
        }


        [HttpGet("GetItemPendingSale")]
        public async Task<IActionResult> GetItemPendingSale([FromQuery] string? username, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                string? creatorHash = null;

                if (username != null)
                {
                    creatorHash = recoverUserDAO.GetUserHash(username);
                }

                // Retrieve the pending receipts with associated item information
                var (receipts, totalCount) = await itemBuyingDAO.GetPendingReceiptsWithItemInfo(creatorHash, pageNumber, pageSize);

                // Construct the result
                var result = new
                {
                    Receipts = receipts.Select(receipt => new
                    {
                        receipt.ReceiptID,
                        receipt.SKU,
                        receipt.OfferPrice,
                        receipt.Quantity,
                        receipt.Profit,
                        receipt.Revenue,
                        receipt.SaleDate,
                        Item = new
                        {
                            receipt.Item.Name,
                            receipt.Item.Sku,
                            receipt.Item.Price,
                            receipt.Item.StockAvailable,
                            receipt.Item.FirstImage
                        }
                    }),
                    TotalCount = totalCount,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred: " + ex.Message);
            }
        }


        [HttpPost("AcceptPendingSale")]
        public async Task<IActionResult> AcceptPendingSale([FromBody] CraftReceiptModel receipt)
        {
            //Console.WriteLine(receipt);
            if ( string.IsNullOrWhiteSpace(receipt.SKU) || receipt.ReceiptID == 0 || receipt.Quantity <= 0)
            {
                return BadRequest("Invalid Sku provided.");
            }

            ItemCreationModel item = await itemCreationDAO.GetItemBySkuAsync(receipt.SKU, false);
            if (receipt.Quantity > item.StockAvailable)
            {
                return BadRequest("Cannot accept the sale, Seller's stock is lower than buyer's quantity");
            }

            CraftReceiptModel? receipt2 = await itemBuyingDAO.GetReceiptByIDAsync(receipt.ReceiptID);
            bool isSuccess = await craftReceiptService.acceptPendingSale(receipt2!);

            if (isSuccess)
            {
                return Ok(new { message = "Sale is successfully accepted." });
            }
            else
            {
                return NotFound(new { message = "cannot accept the sale, error!!!" });
            }
        }


        [HttpDelete("DeclinePendingSale")]
        public async Task<IActionResult> DeclinePendingSale([FromBody] CraftReceiptModel receipt)
        {
            if (receipt == null || receipt.ReceiptID == 0)
            {
                return BadRequest("Invalid Receipt ID.");
            }

            bool isSuccess = await craftReceiptService.declinePendingSale(receipt);

            if (isSuccess)
            {
                return Ok(new { message = "Sale is successfully declined and removed." });
            }
            else
            {
                return NotFound(new { message = "cannot decline the sale, error!!!" });
            }
        }


    }
}
