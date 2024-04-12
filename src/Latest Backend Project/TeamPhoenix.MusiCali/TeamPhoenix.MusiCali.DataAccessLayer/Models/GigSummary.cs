namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class GigSummary
    {
        public string Username { get; set; } = string.Empty;
        public string GigName { get; set; } = string.Empty;
        public DateTime DateOfGig { get; set; }

        public GigSummary(string username, string gigname, DateTime gigDate)
        {
            Username = username;
            GigName = gigname;
            DateOfGig = gigDate;
        }
    }
}
