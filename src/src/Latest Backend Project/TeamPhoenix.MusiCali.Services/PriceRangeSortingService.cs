using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TeamPhoenix.MusiCali.Logging;

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

    public async Task<(HashSet<Item> items, string message, int totalCount)> GetPagedFilteredItems(ItemQueryParameters parameters)
    {
        string? userHash = null;
        string logContext = $"Username: Anonymous, Page: {parameters.PageNumber}, PageSize: {parameters.PageSize}, NameFilter: {parameters.Name}, BottomPrice: {parameters.BottomPrice}, TopPrice: {parameters.TopPrice}";

        try
        {
            // Create an ItemQuery object from the parameters
            ItemQueryParameters query = new ItemQueryParameters
            {
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                Name = parameters.Name,
                BottomPrice = parameters.BottomPrice,
                TopPrice = parameters.TopPrice
            };

            // Pass the query object to FetchPagedItems
            var result = await _dataAccessLayer.FetchPagedItems(query);
            var items = result.items;
            var totalCount = result.totalCount;

            if (items == null || items.Count == 0)
            {
                string errorMessage = "No items found with the given criteria.";
                _loggerService.CreateLog(userHash!, LogLevel.Error.ToString(), "Data", errorMessage);
                return (new HashSet<Item>(), errorMessage, totalCount);
            }

            if (items.Count > parameters.PageSize)
            {
                string errorMessage = "Incorrect sorting format shown to the user.";
                _loggerService.CreateLog(userHash!, LogLevel.Error.ToString(), "View", errorMessage);
                return (new HashSet<Item>(), errorMessage, totalCount);
            }

            string successMessage = $"Fetched {items.Count} items out of {totalCount}.";
            _loggerService.CreateLog(userHash!, LogLevel.Information.ToString(), "Item Retrieval", successMessage + " " + logContext);
            return (items, successMessage, totalCount);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Error fetching items: {ex.Message}.";
            _loggerService.CreateLog(userHash!, LogLevel.Error.ToString(), "Item Retrieval", errorMessage + " " + logContext);
            return (new HashSet<Item>(), errorMessage, 0);
        }
    }

    public async Task<(int count, string message)> GetTotalItemCount()
    {
        string? userHash = null;
        try
        {
            int itemCount = await _dataAccessLayer.CountItems();
            if (itemCount < 0)
            {
                string errorMessage = "Invalid total item count received.";
                _loggerService.CreateLog(userHash!, LogLevel.Error.ToString(), "View", "Invalid Page Count shown to user.");
                return (0, errorMessage);
            }

            string successMessage = $"Total items count: {itemCount}.";
            _loggerService.CreateLog(userHash!, LogLevel.Information.ToString(), "Item Count", successMessage);
            return (itemCount, successMessage);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Error counting items: {ex.Message}.";
            _loggerService.CreateLog(userHash!, LogLevel.Error.ToString(), "Item Count", errorMessage);
            return (0, errorMessage);
        }
    }
}