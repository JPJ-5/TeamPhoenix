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
    public async Task<IActionResult> GetPagedFilteredItems([FromQuery] ItemQueryParameters parameters)
    {
        try
        {
            // You need to create an instance of ItemFilterParameters from the query parameters
            ItemQueryParameters filterParameters = new ItemQueryParameters
            {
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                Name = parameters.Name,
                BottomPrice = parameters.BottomPrice,
                TopPrice = parameters.TopPrice
            };

            // Now pass this instance to your service
            var result = await _itemService.GetPagedFilteredItems(filterParameters);
            var items = result.items ?? new HashSet<Item>();
            var totalItemCount = result.totalCount;

            return Ok(new ApiResponse<object>(new
            {
                TotalCount = totalItemCount,
                Items = items,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            }));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new ApiResponse<object>("An error occurred while fetching the items: " + ex.Message));
        }
    }
}