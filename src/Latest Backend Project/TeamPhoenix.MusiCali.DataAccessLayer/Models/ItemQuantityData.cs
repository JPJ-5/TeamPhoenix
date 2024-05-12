namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class ItemQuantityData
    {
        public string itemName;
        public decimal quantity;
        public ItemQuantityData(string item, decimal itemSold)
        {
            itemName = item;
            quantity = itemSold;
        }
    }
}
