using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    private readonly ItemService _itemService;
    private readonly IConfiguration configuration;

    public ItemController(ItemService itemService, IConfiguration configuration)
    {
        _itemService = itemService;
        this.configuration = configuration;
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

            // Retrieve both items and total count from the service
            var result = await _itemService.GetPagedFilteredItems(pageNumber, pageSize, name, bottomPrice, topPrice);
            var items = result.items ?? new HashSet<Item>(); // Ensure items is never null
            var totalItemCount = result.totalCount; // Directly use the count provided by the service

            // Construct the response with item information
            return Ok(new
            {
                TotalCount = totalItemCount,
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred while fetching the items: " + ex.Message);
        }
    }

}