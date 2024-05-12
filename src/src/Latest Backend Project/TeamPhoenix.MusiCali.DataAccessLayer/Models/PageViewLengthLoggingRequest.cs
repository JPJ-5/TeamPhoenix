namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class PageViewLengthLoggingRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public int PageLength { get; set; }
    }
}
