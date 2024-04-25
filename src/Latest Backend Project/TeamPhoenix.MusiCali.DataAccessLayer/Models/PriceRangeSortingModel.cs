public class Item
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? SKU { get; set; } // Assuming SKU is a string property

    public override bool Equals(object? obj)
    {
        // Override to use SKU for equality
        return obj is Item other && SKU == other.SKU;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Price);
    }
}