using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetItemDetailController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ItemCreationDAO itemCreationDAO;

        public GetItemDetailController(IAmazonS3 s3Client, IConfiguration configuration)
        {
            this.configuration = configuration;
            itemCreationDAO = new ItemCreationDAO(this.configuration, s3Client);

        }
        [HttpGet]
        public async Task<ActionResult<ItemCreationModel>> GetItemBySku([FromQuery] string sku)
        {
            try
            {
                var item = await itemCreationDAO.GetItemBySkuAsync(sku);
                if (item == null)
                {
                    return NotFound("Item not found.");
                }

                return Ok(item);
            }
            catch (System.Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }

}
