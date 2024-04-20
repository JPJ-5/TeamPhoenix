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
        public int gigID { get; set; }
        public bool isAlreadyInterested { get; set; } = false;

        public GigSummary(string username, string gigname, DateTime gigDate, string gigLocation, string gigPay, string gigDesc, int gigID, bool interest)
        {
            Username = username;
            GigName = gigname;
            DateOfGig = gigDate;
            Location = gigLocation;
            Pay = gigPay;
            Description = gigDesc;
            this.gigID = gigID;
            isAlreadyInterested = interest;
        }
    }
}