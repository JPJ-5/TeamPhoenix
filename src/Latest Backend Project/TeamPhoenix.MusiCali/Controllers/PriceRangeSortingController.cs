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
    public async Task<IActionResult> SortItems(decimal bottomPrice, decimal topPrice)
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
        if (items == null || !items.Any())
        {
            return Ok(new ApiResponse<HashSet<Item>>(new HashSet<Item>(), "No items found matching your search."));
        }

        return Ok(new ApiResponse<HashSet<Item>>(items));
    }

    [HttpGet("api/items")]
    public async Task<IActionResult> GetItems()
    {
        var items = await _itemService.GetAllItems();
        if (items == null || items.Count == 0)
        {
            return Ok(new ApiResponse<HashSet<Item>>(new HashSet<Item>(), "No items found."));
        }

        return Ok(new ApiResponse<HashSet<Item>>(items));
    }

    [HttpGet("api/pagedItems")]
    public async Task<IActionResult> GetPagedItems(int pageNumber, int pageSize)
    {
        try
        {
            var items = await _itemService.GetPagedItems(pageNumber, pageSize);
            int totalItemCount = await _itemService.GetTotalItemCount();
            return Ok(new
            {
                TotalCount = totalItemCount,
                Items = items ?? new List<Item>(), // Ensure this never returns null
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