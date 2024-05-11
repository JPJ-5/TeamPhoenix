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
        public async Task<IActionResult> GetItems([FromQuery] string? listed, [FromQuery] string? offerable, [FromQuery] string? username, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                string? creatorHash = null;

                if (username != null)
                {
                    creatorHash = recoverUserDAO.GetUserHash(username);
                }

                // Assuming GetItemList now returns a tuple of (HashSet<PaginationItemModel>, int)
                var (items, totalCount) = await itemPaginationDAO.GetItemListAndCountPagination(listed, offerable, creatorHash, pageNumber, pageSize);

                // Create a simple object or dictionary to send the items and the total count
                var result = new
                {
                    Items = items,
                    TotalCount = totalCount,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest("An error occurred: " + ex.Message);
            }
        }


       

    }
}
