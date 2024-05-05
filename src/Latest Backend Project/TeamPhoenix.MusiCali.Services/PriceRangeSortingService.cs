using TeamPhoenix.MusiCali.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class ItemService
{
    private readonly DataAccessLayer _dataAccessLayer;
    private readonly LoggerService _loggerService;
    private readonly IConfiguration _configuration;

    public ItemService(DataAccessLayer dataAccessLayer, IConfiguration configuration)
    {
        _dataAccessLayer = dataAccessLayer;
        _loggerService = new LoggerService(configuration);
        _configuration = configuration;
    }

    public async Task<(HashSet<Item> items, int totalCount)> GetPagedFilteredItems(int pageNumber, int pageSize, string? name = null, decimal? bottomPrice = null, decimal? topPrice = null)
    {
        string userHash = "e12a8f14d3623f5206c060b0d1fba3d7105afc5062d13173aa17866d3b53b0d6";
        string logContext = $"Username: {"Anonymous"}, Page: {pageNumber}, PageSize: {pageSize}, NameFilter: {name}, BottomPrice: {bottomPrice}, TopPrice: {topPrice}";

        try
        {
            // Fetch items and total count
            var result = await _dataAccessLayer.FetchPagedItems(pageNumber, pageSize, name, bottomPrice, topPrice);
            var items = result.items;
            var totalCount = result.totalCount;

            // Log an error if no items are found
            if (items == null || items.Count == 0)
            {
                _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "Data", "System fails to show the sorting result with valid available data.");
                return (new HashSet<Item>(), totalCount); // Return empty set and total count
            }

            // Log error if items exceed the page size limit
            if (items.Count > pageSize)
            {
                _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "View", "Wrong sorting format is shown to user.");
                throw new Exception("Results are not shown in the correct format.");
            }

            _loggerService.CreateLog(userHash, LogLevel.Information.ToString(), "Item Retrieval", $"Fetched {items.Count} items out of {totalCount}. " + logContext);
            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "Item Retrieval", $"Error fetching items: {ex.Message}. " + logContext);
            throw;
        }
    }

    public async Task<int> GetTotalItemCount()
    {
        string userHash = "e12a8f14d3623f5206c060b0d1fba3d7105afc5062d13173aa17866d3b53b0d6";
        try
        {
            int itemCount = await _dataAccessLayer.CountItems();
            if (itemCount < 0)
            {
                _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "View", "Invalid Page Count shown to user.");
                throw new Exception("Invalid total item count received.");
            }
            _loggerService.CreateLog(userHash, LogLevel.Information.ToString(), "Item Count", $"Total items count: {itemCount}");
            return itemCount;
        }
        catch (Exception ex)
        {
            _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "Item Count", $"Error counting items: {ex.Message}");
            throw;
        }
    }
}