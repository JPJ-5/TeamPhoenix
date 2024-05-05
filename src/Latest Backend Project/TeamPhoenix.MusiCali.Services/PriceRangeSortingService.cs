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

    public async Task<(HashSet<Item> items, string message, int totalCount)> GetPagedFilteredItems(int pageNumber, int pageSize, string? name = null, decimal? bottomPrice = null, decimal? topPrice = null)
    {
        string userHash = "e12a8f14d3623f5206c060b0d1fba3d7105afc5062d13173aa17866d3b53b0d6";
        string logContext = $"Username: {"Anonymous"}, Page: {pageNumber}, PageSize: {pageSize}, NameFilter: {name}, BottomPrice: {bottomPrice}, TopPrice: {topPrice}";

        try
        {
            var result = await _dataAccessLayer.FetchPagedItems(pageNumber, pageSize, name, bottomPrice, topPrice);
            var items = result.items;
            var totalCount = result.totalCount;

            if (items == null || items.Count == 0)
            {
                string errorMessage = "No items found with the given criteria.";
                _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "Data", errorMessage);
                return (new HashSet<Item>(), errorMessage, totalCount);
            }

            if (items.Count > pageSize)
            {
                string errorMessage = "Incorrect sorting format shown to the user.";
                _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "View", errorMessage);
                return (new HashSet<Item>(), errorMessage, totalCount);
            }

            string successMessage = $"Fetched {items.Count} items out of {totalCount}.";
            _loggerService.CreateLog(userHash, LogLevel.Information.ToString(), "Item Retrieval", successMessage + " " + logContext);
            return (items, successMessage, totalCount);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Error fetching items: {ex.Message}.";
            _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "Item Retrieval", errorMessage + " " + logContext);
            return (new HashSet<Item>(), errorMessage, 0);
        }
    }

    public async Task<(int count, string message)> GetTotalItemCount()
    {
        string userHash = "e12a8f14d3623f5206c060b0d1fba3d7105afc5062d13173aa17866d3b53b0d6";
        try
        {
            int itemCount = await _dataAccessLayer.CountItems();
            if (itemCount < 0)
            {
                string errorMessage = "Invalid total item count received.";
                _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "View", "Invalid Page Count shown to user.");
                return (0, errorMessage);
            }

            string successMessage = $"Total items count: {itemCount}.";
            _loggerService.CreateLog(userHash, LogLevel.Information.ToString(), "Item Count", successMessage);
            return (itemCount, successMessage);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Error counting items: {ex.Message}.";
            _loggerService.CreateLog(userHash, LogLevel.Error.ToString(), "Item Count", errorMessage);
            return (0, errorMessage);
        }
    }
}