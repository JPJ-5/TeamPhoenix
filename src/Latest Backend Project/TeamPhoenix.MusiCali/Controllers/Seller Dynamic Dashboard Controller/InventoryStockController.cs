using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
[ApiController]
[Route("SellerDashboard")]
public class InventoryStockController : ControllerBase
{
    private readonly InventoryStockService itemService;

    public InventoryStockController(IConfiguration configuration)
    {
        itemService = new InventoryStockService(configuration);
    }

    [HttpGet("api/GetInventoryStock")]
    public async Task<IActionResult> GetInventoryStock([FromHeader]string username)
    {
        try
        {
            var stockList = await itemService.RequestInventoryStockList(username);
            
            return stockList.Count > 0 ? Ok(stockList) : NotFound("No stock items found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}
