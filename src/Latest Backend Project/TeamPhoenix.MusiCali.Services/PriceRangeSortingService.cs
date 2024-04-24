public class ItemService
{
    private readonly DataAccessLayer _dataAccessLayer;

    public ItemService(DataAccessLayer dataAccessLayer)
    {
        _dataAccessLayer = dataAccessLayer;
    }

    // Fetch sorted items by price range with pagination
    public async Task<HashSet<Item>> SortItemsByPriceRange(decimal topPrice, decimal bottomPrice)
    {
        return await _dataAccessLayer.FetchItems(topPrice, bottomPrice);
    }

    // Fetch items by name or other search criteria with pagination
    public async Task<HashSet<Item>> SearchItemsByName(string query)
    {
        return await _dataAccessLayer.FetchItemsByName(query);
    }

    public async Task<HashSet<Item>> GetAllItems()
    {
        return await _dataAccessLayer.GetAllItems();
    }

    public async Task<List<Item>> GetPagedItems(int pageNumber, int pageSize)
    {
        return await _dataAccessLayer.FetchPagedItems(pageNumber, pageSize);
    }

    public async Task<int> GetTotalItemCount()
    {
        return await _dataAccessLayer.CountItems();
    }
}