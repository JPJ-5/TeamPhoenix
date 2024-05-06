using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

public class InventoryStockController : ControllerBase
{
    private readonly InventoryStockService _itemService;

    public InventoryStockController(IConfiguration configuration)
    {
        _itemService = new InventoryStockService(configuration);
    }

    [HttpGet("api/inventorystock")]
    public async Task<IActionResult> GetInventoryStock([FromHeader]string username)
    {
        try
        {
            var stockList = await _itemService.RequestInventoryStockList(username);
            
            return stockList.Count > 0 ? Ok(stockList) : NotFound("No stock items found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}
