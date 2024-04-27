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

    public async Task<HashSet<Item>> GetPagedFilteredItems(int pageNumber, int pageSize, string? name = null, decimal? bottomPrice = null, decimal? topPrice = null)
    {
        string userHash = "e12a8f14d3623f5206c060b0d1fba3d7105afc5062d13173aa17866d3b53b0d6";
        string logContext = $"Username: {"Anonymous"}, Page: {pageNumber}, PageSize: {pageSize}, NameFilter: {name}, BottomPrice: {bottomPrice}, TopPrice: {topPrice}";

        try
        {
            var items = await _dataAccessLayer.FetchPagedItems(pageNumber, pageSize, name, bottomPrice, topPrice);
            _loggerService.CreateLog(userHash, LogLevel.Information.ToString(), "Item Retrieval", $"Fetched {items.Count} items. " + logContext);
            return items;
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