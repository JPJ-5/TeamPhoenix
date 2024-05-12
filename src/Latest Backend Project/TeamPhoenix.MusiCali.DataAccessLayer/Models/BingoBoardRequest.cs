namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class BingoBoardRequest
    {
        public int NumberOfGigs { get; set; } = 10;
        public string Username { get; set; } = "";
        public int Offset { get; set; } = 0;

    }
}
