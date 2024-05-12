namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class GigView
    {
        public string Username { get; set; }
        public string GigName { get; set; }
        public DateTime DateOfGig { get; set; }
        public bool Visibility { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Pay { get; set; }
        public GigView(string userName, string gigName, DateTime dateOfGig, bool visibility, string description, string location, string pay)
        {

            Username = userName;
            GigName = gigName;
            DateOfGig = dateOfGig;
            Visibility = visibility;
            Description = description;
            Location = location;
            Pay = pay;
        }
    }
}
