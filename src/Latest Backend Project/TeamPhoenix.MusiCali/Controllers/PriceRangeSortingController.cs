using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    // Dependencies: service layer to handle data and configuration
    private readonly ItemService _itemService;
    private readonly IConfiguration configuration;

    // Constructor to inject dependencies
    public ItemController(ItemService itemService, IConfiguration configuration)
    {
        _itemService = itemService;
        this.configuration = configuration;
    }

    // GET endpoint to fetch filtered and paginated items
    [HttpGet("api/pagedFilteredItems")]
    public async Task<IActionResult> GetPagedFilteredItems(int pageNumber, int pageSize, string? name = null, decimal? bottomPrice = null, decimal? topPrice = null)
    {
        try
        {
            // Validation: Ensure top price is not less than bottom price
            if (topPrice.HasValue && bottomPrice.HasValue && topPrice < bottomPrice)
            {
                return BadRequest(new ApiResponse<object>("Top price cannot be less than bottom price."));
            }

            // Fetch filtered items and their total count from the service layer
            var result = await _itemService.GetPagedFilteredItems(pageNumber, pageSize, name, bottomPrice, topPrice);
            var items = result.items ?? new HashSet<Item>(); // Ensure that items are not null
            var totalItemCount = result.totalCount; // Total count fetched alongside items

            // Construct and return response with pagination details
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
            // Log and return an error response in case of an exception
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred while fetching the items: " + ex.Message);
        }
    }
}