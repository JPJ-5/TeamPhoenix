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

    [HttpGet("api/sort")]
    public async Task<IActionResult> SortItems(decimal topPrice = 1000000M, decimal bottomPrice = 0M)
    {
        if (topPrice < bottomPrice)
        {
            return BadRequest(new ApiResponse<object>("Top price cannot be less than bottom price."));
        }

        var items = await _itemService.SortItemsByPriceRange(topPrice, bottomPrice);
        if (items == null || items.Count == 0)
        {
            return Ok(new ApiResponse<HashSet<Item>>(new HashSet<Item>(), "No items found within the specified price range."));
        }

        return Ok(new ApiResponse<HashSet<Item>>(items));
    }

    [HttpGet("api/search")]
    public async Task<IActionResult> SearchItems(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(new ApiResponse<object>("Query cannot be empty."));
        }

        var items = await _itemService.SearchItemsByName(query);
        if (items.Count == 0)
        {
            return Ok(new ApiResponse<HashSet<Item>>(new HashSet<Item>(), "No items found matching your search."));
        }

        return Ok(new ApiResponse<HashSet<Item>>(items));
    }
}
