namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class GigSummary
    {
        public string Username { get; set; } = string.Empty;
        public string GigName { get; set; } = string.Empty;
        public DateTime DateOfGig { get; set; }
        public string Location { get; set; }
        public string Pay { get; set; }

        public GigSummary(string username, string gigname, DateTime gigDate, string gigLocation, string gigPay)
        {
            Username = username;
            GigName = gigname;
            DateOfGig = gigDate;
            Location = gigLocation;
            Pay = gigPay;
        }
    }
}