public class Item
{
    public string? Name { get; set; }
    public decimal Price { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is Item other)
        {
            return Name == other.Name && Price == other.Price;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Price);
    }
}