using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using TeamPhoenix.MusiCali.Security;

namespace TeamPhoenix.MusiCali.Services
{
    public class CraftReceiptService
    {
        private readonly ItemBuyingDAO? itemBuyingDAO;
        private readonly ItemCreationDAO? itemCreationDAO;
        private readonly IConfiguration configuration;
        private readonly LoggerService loggerService;

        public CraftReceiptService(IConfiguration configuration, IAmazonS3 s3Client)
        {
            this.configuration = configuration;
            loggerService = new LoggerService(this.configuration);
            itemBuyingDAO = new ItemBuyingDAO(this.configuration);
            itemCreationDAO = new ItemCreationDAO(this.configuration, s3Client);
        }

        public bool IsValidPrice(decimal price)
        {
            // Convert the decimal to a string with a culture-invariant format, ensuring no unnecessary decimal places
            string decimalString = price.ToString("0.##", CultureInfo.InvariantCulture);

            // Regular expression to match exactly 5 digits or 5 digits with 2 after the decimal point
            string pattern = @"^\d{1,7}(\.\d{2})?$";

            // Returns true if the input matches the pattern, false otherwise
            return Regex.IsMatch(decimalString, pattern);
        }


        public async Task<bool> CreateAReceiptAsync( string buyerHash, string sku, bool pendingSale, decimal offerPrice, int quantity)
        {
            if ((buyerHash == null) || (buyerHash.Length != 64) )
            {
                return false;
                throw new ArgumentException("Invalid buyer username provided. Retry again or contact system administrator");
            }
            if (!(itemCreationDAO!.IsSkuDuplicate(sku)))           // chekc if the item sku is exist in database
            {
                return false;
                throw new ArgumentException("Invalid sku provided. Retry again or contact system administrator");
            }
            ItemCreationModel item = await itemCreationDAO.GetItemBySkuAsync(sku, false);
            string creatorHash = itemBuyingDAO!.GetUserHashBySku(sku);


            if (!IsValidPrice(offerPrice))
            {
                return false;
                throw new ArgumentException("Invalid item price provided. Retry again or contact system administrator");
            }
            if (quantity < 0 || quantity > 1000 || quantity > item.StockAvailable)
            {
                Console.WriteLine($"Not enough stock for this sale or buyer input stock is invalid");
                return false;
                throw new ArgumentException("Invalid item stock provided. Retry again or contact system administrator");
            }

            //calculate profit, revenue
            decimal revenue;
            decimal profit;

            if (pendingSale)
            {
                revenue = offerPrice * quantity;
                profit = (offerPrice - item.ProductionCost) * quantity;
            }
            else
            {
                revenue = item.Price * quantity;
                profit = (item.Price - item.ProductionCost) * quantity; 
            }

            
            try
            {
                CraftReceiptModel newReceipt = new CraftReceiptModel( creatorHash, buyerHash, sku, item.Price, offerPrice, profit, revenue, quantity, pendingSale);
                int newstock = item.StockAvailable - quantity;
                string buyerEmail = itemBuyingDAO!.GetEmailByUserHash(buyerHash);
                string sellerEmail = itemBuyingDAO.GetEmailByUserHash(creatorHash);
                string messageToBuyer = " Oops, there is error with your purchase. We may run out of stock or the seller decline the sale, please contact seller at {sellerEmail}";
                string messageToSeller= " Oops, there is error with your sale, check you item stock{item.Name} , sku as {sku}, or contact the buyer at {buyerEmail}";

                if (!(await itemBuyingDAO.InsertRecieptTable(newReceipt))) // insert receipt and update stock available
                {
                    SendConfirmationEmail(buyerEmail, "Sale Confirmation CreaftVerify", messageToBuyer);
                    SendConfirmationEmail(sellerEmail, "Sale Confirmation CreaftVerify", messageToSeller);
                    loggerService.CreateLog(creatorHash, "Warnning", "View", "Receipt making failure");
                    throw new Exception("Unable To insert to receipt table or update item stock available");
                }
                
                if (pendingSale == false)         // if the receipt is offerable, dont reduce stock number. waiting for seller acceptance.
                {
                    bool deductStockAvailable = await itemCreationDAO.UpdateStockAvailable(sku, newstock);              
                }
                string itemName = item.Name;
                decimal itemPrice = item.Price;
                string sellerContact = item.SellerContact;

                messageToBuyer = $"Thank you for your interest in buying the item, {itemName}. The price for this item is {itemPrice}. For further information to make a purchase, please contact the seller at contact info as: {sellerContact}.";
                messageToSeller = $"Thank you for selling your item, {itemName}. The price for this item is {itemPrice}. For further information to finish a sale, please contact the buyer at seller contact info as: {buyerEmail}.";
                SendConfirmationEmail(buyerEmail, "Sale Confirmation CreaftVerify", messageToBuyer);
                SendConfirmationEmail(sellerEmail, "Sale Confirmation CreaftVerify", messageToSeller);
                loggerService.CreateLog(creatorHash, "Info", "View", "Creating new Sale Receipt");
                
                
            }
            catch (SqlException ex1)
            {
                Console.WriteLine($"Error creating new Sale Receipt in sql exception: {ex1.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating new Sale Receipt: {ex.Message}");
                return false;
            }


            return true;
        }


        public bool SendConfirmationEmail(string email, string subject, string message)
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
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = message;

                // Send the email
                smtpClient.Send(mailMessage);

                Console.WriteLine($"Confirmation email sent to {email}. Please check your email.");
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
