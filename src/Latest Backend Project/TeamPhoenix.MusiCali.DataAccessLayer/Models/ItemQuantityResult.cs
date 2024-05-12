namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class ItemQuantityResult
    {
        public List<ItemQuantityData>? Values { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public ItemQuantityResult(List<ItemQuantityData>? itemQuantity, String message, bool result)
        {
            Values = itemQuantity;
            ErrorMessage = message;
            Success = result;
        }
    }
}
