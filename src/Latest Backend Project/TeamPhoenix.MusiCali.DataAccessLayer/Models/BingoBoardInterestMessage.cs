namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class BingoBoardInterestMessage
    {
        public string returnMsg { get; set; } = "Error";
        public bool returnSuccess { get; set; } = false;
        public BingoBoardInterestMessage(string msg, bool succ)
        { 
            this.returnMsg = msg;
            this.returnSuccess = succ;
        }
    }
}
