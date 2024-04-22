public class ItemService
{
    private readonly DataAccessLayer _dataAccessLayer;

    public ItemService(DataAccessLayer dataAccessLayer)
    {
        _dataAccessLayer = dataAccessLayer;
    }

    public async Task<HashSet<Item>> SortItemsByPriceRange(decimal topPrice, decimal bottomPrice)
    {
        return await _dataAccessLayer.FetchItems(topPrice, bottomPrice);
    }
}