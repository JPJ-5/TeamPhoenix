namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class PageViewLengthResult
    {
        public List<PageViewLengthData>? Values { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public PageViewLengthResult(List<PageViewLengthData>? pageViewLength, String message, bool result)
        {
            Values = pageViewLength;
            ErrorMessage = message;
            Success = result;
        }
    }
}
