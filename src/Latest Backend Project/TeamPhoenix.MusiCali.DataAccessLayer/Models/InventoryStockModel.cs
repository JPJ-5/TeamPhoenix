public class InventoryStockModel
{
    public string? Name { get; set; }
    public string? SKU { get; set; }
    public int StockAvailable { get; set; }
    public decimal? Price { get; set; }
    public DateTime? DateCreated { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is InventoryStockModel other)
        {
            return Name == other.Name && SKU == other.SKU
                && StockAvailable == other.StockAvailable
                && Price == other.Price
                && DateCreated == other.DateCreated;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, SKU, StockAvailable, Price, DateCreated);
    }
}