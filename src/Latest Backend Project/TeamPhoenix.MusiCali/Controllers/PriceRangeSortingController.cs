using Microsoft.AspNetCore.Mvc;

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
            return BadRequest("Top price cannot be less than bottom price.");
        }

        var items = await _itemService.SortItemsByPriceRange(topPrice, bottomPrice);
        if (items == null || items.Count == 0)
        {
            return NotFound("No items found within the specified price range.");
        }

        return Ok(items);
    }
}