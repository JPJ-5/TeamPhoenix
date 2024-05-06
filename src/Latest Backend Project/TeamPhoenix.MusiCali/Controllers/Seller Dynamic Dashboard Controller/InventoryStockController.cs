using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using TeamPhoenix.MusiCali.Security;
[ApiController]
[Route("SellerDashboard")]
public class InventoryStockController : ControllerBase
{
    private readonly InventoryStockService itemService;
    private AuthenticationSecurity authentication;

    public InventoryStockController(IConfiguration configuration)
    {
        itemService = new InventoryStockService(configuration);
        authentication = new AuthenticationSecurity(configuration);
    }

    [HttpGet("api/GetInventoryStock")]
    public async Task<IActionResult> GetInventoryStock([FromHeader]string username)
    {
        try
        {
            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var role = authentication.getScopeFromToken(accessToken!);
            var user = authentication.getUserFromToken(accessToken!);

            if (!string.IsNullOrEmpty(role) && authentication.CheckIdRoleExisting(user, role))
            {
                var stockList = await itemService.RequestInventoryStockList(username);

                return stockList.Count > 0 ? Ok(stockList) : BadRequest("No stock items found.");
            }
            else
            {
                throw new Exception("Invalid Identity. Please Contact The System Administrator.");
            }
            
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}
