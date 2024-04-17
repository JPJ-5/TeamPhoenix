namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class GigSummary
    {
        public string Username { get; set; } = string.Empty;
        public string GigName { get; set; } = string.Empty;
        public DateTime DateOfGig { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Pay { get; set; } = string.Empty ;
        public string Description {  get; set; } = string.Empty ;

        public GigSummary(string username, string gigname, DateTime gigDate, string gigLocation, string gigPay, string gigDesc)
        {
            Username = username;
            GigName = gigname;
            DateOfGig = gigDate;
            Location = gigLocation;
            Pay = gigPay;
            Description = gigDesc;

        }
    }
}