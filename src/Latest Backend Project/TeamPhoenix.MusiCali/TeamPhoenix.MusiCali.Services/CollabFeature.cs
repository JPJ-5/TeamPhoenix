using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using MySqlX.XDevAPI.Common;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Net;
using TeamPhoenix.MusiCali.DataAccessLayer; // Import the namespace where CollabFeatureDAL is defined
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Services
{
    public class CollabFeature
    {
        public static Result CreateCollabRequest(string senderUsername, string receiverUsername)
        {
            
            Result result = new Result();

            try
            {
                // Get sender and receiver email addresses using CollabFeatureDAL
                string senderEmail = CollabFeatureDAL.GetEmailByUsername(senderUsername);
                string receiverEmail = CollabFeatureDAL.GetEmailByUsername(receiverUsername);
                
                // Insert a new collab record into the Collab table using CollabFeatureDAL
                Result _dalResult = CollabFeatureDAL.InsertCollab(senderUsername, receiverUsername, senderEmail, receiverEmail);

                if (_dalResult.Success == false){

                    result.ErrorMessage = _dalResult.ErrorMessage;
                    return result;
                }

                //var receiverEmail = CollabFeatureDAL.GetEmailByUsername(receiverUsername);
                var isEmailSent = SendCollabEmail(receiverEmail, senderUsername);

                if (isEmailSent == false){

                    result.HasError = true;
                    result.ErrorMessage = "Request Sent, But Email Hasn't Been Sent";
                }

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
        

        //function for alerting user that their request was sent
        public static bool SendCollabEmail(string email, string senderUsername)
        {
            try
            {
                // Your email configuration
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587; // Use 587 for TLS
                string smtpUsername = "themusicali.otp@gmail.com";
                string smtpPassword = "wqpgjtdy xnsjcsvm";

                // Create a new SmtpClient with the specified configuration
                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
                smtpClient.EnableSsl = true; // Use SSL/TLS
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                // Create the email message
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(smtpUsername);
                mailMessage.To.Add(email); //email where the message is being sent to
                mailMessage.Subject = "MusiCali Collab Request Received";
                mailMessage.Body = $"You have a new collab request!: {senderUsername}";

                // Send the email
                smtpClient.Send(mailMessage);

                Console.WriteLine($"Confirmation email sent to {email}. Please check your email for the OTP.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending confirmation email: {ex.Message}");
                return false;
            }
        }
    }
}
