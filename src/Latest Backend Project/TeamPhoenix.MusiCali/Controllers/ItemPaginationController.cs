using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemPaginationController : ControllerBase
    {

        private readonly ItemPaginationDAO itemPaginationDAO;
        //private readonly IConfiguration configuration;
        private RecoverUserDAO recoverUserDAO;

        public ItemPaginationController( IAmazonS3 s3Client, IConfiguration configuration)
        {
            itemPaginationDAO = new ItemPaginationDAO(s3Client, configuration);
            recoverUserDAO = new RecoverUserDAO(configuration);
        }

        [HttpGet]
        public async Task<IActionResult> GetItems([FromQuery] string? listed, [FromQuery] string? offerable, [FromQuery] string? username, [FromQuery] int pageNumber , [FromQuery] int pageSize )
        {
            try
            {
                string? creatorHash = null;
                
                
                if(username != null)
                {
                    creatorHash = recoverUserDAO.GetUserHash(username!);
                }
                
                
                var items = await itemPaginationDAO.GetItemList(listed, offerable, creatorHash!);

                Console.WriteLine(items);
                var pageList = PageList<PaginationItemModel>.CreatePagination(items.AsQueryable(), pageNumber, pageSize);
                return Ok(pageList);
            }
            catch (System.Exception ex)
            {
                return BadRequest("An error occurred: " + ex.Message);
            }
        }
    }
}
