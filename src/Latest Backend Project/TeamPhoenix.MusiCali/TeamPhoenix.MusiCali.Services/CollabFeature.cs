using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using MySqlX.XDevAPI.Common;
using MySql.Data.MySqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer; // Import the namespace where CollabFeatureDAL is defined
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Services
{
    public class CollabFeature
    {
        public static Result SendCollabEmail(string senderUsername, string receiverUsername)
        {
            
            Result result = new Result();

            try
            {
                // Get sender and receiver email addresses using CollabFeatureDAL
                string senderEmail = CollabFeatureDAL.GetEmailByUsername(senderUsername);
                string receiverEmail = CollabFeatureDAL.GetEmailByUsername(receiverUsername);

                // Insert a new collab record into the Collab table using CollabFeatureDAL
                CollabFeatureDAL.InsertCollab(senderUsername, receiverUsername, senderEmail, receiverEmail);


                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send collab email: " + ex.Message);
            }
        }

        public static Result AcceptCollab(string receiverUsername, string senderUsername)
        {
            try
            {
                // Search for the collab record based on sender and receiver usernames using CollabFeatureDAL
                Result collab = CollabFeatureDAL.AcceptCollabByUsername(senderUsername, receiverUsername);


                if (collab.Success == true)
                {

                    // senderEmail = GetEmailByUsername(collab);
                    // receiverEmail = GetEmailByUsername(collab);
                    // Update the 'Accepted' field to true using CollabFeatureDAL
                    //collab.Success = true;
                    collab = CollabFeatureDAL.AcceptCollabByUsername(senderUsername, receiverUsername);

                    return collab;
                }
                else
                {
                    collab.Success = false;
                    collab.ErrorMessage = "Collab record not found.";

                    return collab;
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to accept collab: " + ex.Message);
            }
        }

        public static CollabData LoadCollabFeature(string username)
        {
            try
            {
                CollabData collabs = new CollabData();
                // Load sent, received, and accepted collabs for the given username using CollabFeatureDAL
                collabs.sentCollabs = CollabFeatureDAL.GetSentCollabsByUsername(username);
                collabs.receivedCollabs = CollabFeatureDAL.GetReceivedCollabsByUsername(username);
                collabs.acceptedCollabs = CollabFeatureDAL.GetAcceptedCollabsByUsername(username);

                return collabs;
            }
            catch (Exception ex)
            {
                throw new Exception("Collab failed to load" + ex.Message);
            }
        }
    }
}
