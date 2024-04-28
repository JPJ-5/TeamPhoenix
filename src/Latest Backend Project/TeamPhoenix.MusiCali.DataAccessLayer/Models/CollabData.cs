namespace TeamPhoenix.MusiCali.DataAccessLayer.Models{



    public class CollabData{

        public List<string> availUsers {get; set;}  
        public List<string> sentCollabs{ get; set; }
        public List<string> receivedCollabs{ get; set; }
        public List<List<string>> acceptedCollabs{ get; set; }
    }
}