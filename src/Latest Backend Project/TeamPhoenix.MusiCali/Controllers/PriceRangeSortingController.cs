using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    private readonly ItemService _itemService;

    public ItemController(ItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet("api/pagedFilteredItems")]
    public async Task<IActionResult> GetPagedFilteredItems(int pageNumber, int pageSize, string? name = null, decimal? bottomPrice = null, decimal? topPrice = null)
    {
        try
        {
            if (topPrice.HasValue && bottomPrice.HasValue && topPrice < bottomPrice)
            {
                return BadRequest(new ApiResponse<object>("Top price cannot be less than bottom price."));
            }

            var items = await _itemService.GetPagedFilteredItems(pageNumber, pageSize, name, bottomPrice, topPrice);
            int totalItemCount = await _itemService.GetTotalItemCount();
            return Ok(new
            {
                TotalCount = totalItemCount,
                Items = items ?? new HashSet<Item>(), // Ensure this never returns null
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while fetching the items: " + ex.Message);
        }
    }
}