namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class GigUpdateModel
    {
        public DateTime DateOfGigOriginal { get; set; }
        public string Username { get; set; } = string.Empty;
        public string GigName { get; set; } = string.Empty;
        public DateTime DateOfGig { get; set; }
        public bool Visibility { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Pay { get; set; } = string.Empty;
    }
}
