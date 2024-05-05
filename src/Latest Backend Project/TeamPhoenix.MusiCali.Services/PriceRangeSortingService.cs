using TeamPhoenix.MusiCali.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class ItemService
{
    // Dependencies: Data Access Layer and Logging service
    private readonly DataAccessLayer _dataAccessLayer;
    private readonly LoggerService _loggerService;
    private readonly IConfiguration _configuration;

    // Constructor to inject dependencies
    public ItemService(DataAccessLayer dataAccessLayer, IConfiguration configuration)
    {
        _dataAccessLayer = dataAccessLayer;
        _loggerService = new LoggerService(configuration);
        _configuration = configuration;
    }

    // Method to fetch paginated and filtered items
    public async Task<(HashSet<Item> items, int totalCount)> GetPagedFilteredItems(int pageNumber, int pageSize, string? name = null, decimal? bottomPrice = null, decimal? topPrice = null)
    {
        // Dummy user hash for logging purposes
        string userHash = "e12a8f14d3623f5206c060b0d1fba3d7105afc5062d13173aa17866d3b53b0d6";
        string logContext = $"Username: {"Anonymous"}, Page: {pageNumber}, PageSize: {pageSize}, NameFilter: {name}, BottomPrice: {bottomPrice}, TopPrice: {topPrice}";

        try
        {
            // Fetch items and their total count from the data access layer
            var result = await _dataAccessLayer.FetchPagedItems(pageNumber, pageSize, name, bottomPrice, topPrice);
            var items = result.items;
            var totalCount = result.totalCount;

            // Log error if no items are found
            if (items == null || items.Count == 0)
            {
                _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "Data", "System fails to show the sorting result with valid available data.");
                return (new HashSet<Item>(), totalCount); // Return an empty set and count
            }

            // Log an error if items exceed the page size limit
            if (items.Count > pageSize)
            {
                _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "View", "Wrong sorting format is shown to user.");
                throw new Exception("Results are not shown in the correct format.");
            }

            // Log successful item retrieval
            _loggerService.CreateLog(userHash, LogLevel.Information.ToString(), "Item Retrieval", $"Fetched {items.Count} items out of {totalCount}. " + logContext);
            return (items, totalCount);
        }
        catch (Exception ex)
        {
            // Log an error if an exception occurs
            _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "Item Retrieval", $"Error fetching items: {ex.Message}. " + logContext);
            throw;
        }
    }

    // Method to return the total item count in the database
    public async Task<int> GetTotalItemCount()
    {
        string userHash = "e12a8f14d3623f5206c060b0d1fba3d7105afc5062d13173aa17866d3b53b0d6";
        try
        {
            // Get the total item count from the data access layer
            int itemCount = await _dataAccessLayer.CountItems();
            if (itemCount < 0)
            {
                _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "View", "Invalid Page Count shown to user.");
                throw new Exception("Invalid total item count received.");
            }

            // Log the total item count
            _loggerService.CreateLog(userHash, LogLevel.Information.ToString(), "Item Count", $"Total items count: {itemCount}");
            return itemCount;
        }
        catch (Exception ex)
        {
            // Log an error if an exception occurs
            _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "Item Count", $"Error counting items: {ex.Message}");
            throw;
        }
    }
}