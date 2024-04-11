using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlX.XDevAPI.Common;

namespace TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.Services
{
    public class CollabFeature
    {
        public Result SendCollabEmail(string username){
            //create collab model based of collab table and input both usernames and emails
             //to get emails use function to search for them using DAL function CREATE TABLE Collab {
            // SenderUsername VARCHAR(255),
            // RecieverUsername VARCHAR(255),
            // Accepted BOOLEAN,
            // SenderEmail VARCHAR(255),
            // RecieverEmail VARCHAR(255)

        }

        public Result AcceptCollab(string RecieverUsername, string senderUsername){
            //search for collab table using both usernames and change bool accept to true
            //use the DAL function
        }

        public Result LoadCollabFeature(string username) {
            //load all 3 Accepted, recieved and sent collab for username

        }
    }
    
}