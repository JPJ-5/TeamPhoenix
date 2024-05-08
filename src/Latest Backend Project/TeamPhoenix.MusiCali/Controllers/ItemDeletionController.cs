using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers.SellerDynamicDashboardControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemDeletionController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private ItemDeletionService itemDeletionService;
        private AuthenticationSecurity authenticationSecurity;
        public ItemDeletionController(IConfiguration configuration, IAmazonS3 s3Client)
        {
            this.configuration = configuration;
            itemDeletionService = new ItemDeletionService(configuration, s3Client);
            authenticationSecurity = new AuthenticationSecurity(configuration);
        }

        [HttpDelete("/DeleteItem")]
        public async Task<IActionResult> DeleteItem([FromHeader] string username, [FromBody] string SKU)
        {
            try
            {
                //var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                //var role = authenticationSecurity.getScopeFromToken(accessToken!);
                //if ((role != string.Empty) && authenticationSecurity.CheckIdRoleExisting(username, role))
                //{
                    if (string.IsNullOrWhiteSpace(SKU))
                    {
                        throw new Exception();
                    }

                    bool deleteItemResult = await itemDeletionService.ItemDeletionRequest(username, SKU);
                    if (deleteItemResult)
                    {
                        return Ok(deleteItemResult);
                    }
                    else
                    {
                        return BadRequest("Item Deletion Request Failed.");
                    }
                //}
                //else
                //{
                //    throw new Exception();
                //}
            }
            catch (Exception)
            {
                return BadRequest("Item Deletion Failed. PLease Wait And Retry Or Contact System Admin.");
            }

        }
    }
}
