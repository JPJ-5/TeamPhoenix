public class ItemService
{
    private readonly DataAccessLayer _dataAccessLayer;

    public ItemService(DataAccessLayer dataAccessLayer)
    {
        _dataAccessLayer = dataAccessLayer;
    }

    public async Task<HashSet<Item>> GetPagedFilteredItems(int pageNumber, int pageSize, string? name = null, decimal? bottomPrice = null, decimal? topPrice = null)
    {
        return await _dataAccessLayer.FetchPagedItems(pageNumber, pageSize, name, bottomPrice, topPrice);
    }

    public async Task<int> GetTotalItemCount()
    {
        return await _dataAccessLayer.CountItems();
    }
}