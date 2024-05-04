using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using TeamPhoenix.MusiCali.Security;
using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using System.Runtime.InteropServices;
using System.Text;


namespace TeamPhoenix.MusiCali.Services
{
    public class ItemCreationService
    {
        //private UserCreationDAO userCreationDAO;
        private readonly IConfiguration configuration;
        private LoggerService loggerService;
        private AuthenticationSecurity authenticationSecurity;
        private Hasher hasher;
        private static Random random = new Random();
        private const string allowedSku = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const string allowedCreatorHash = "";
        private const string allowed = "";
        private const string allowedPrice = "";
        private const string allowedDesc = "";
        private const string allowedCost = "";
        private const string allowedSellerContact = "";
        

        public ItemCreationService(IConfiguration configuration)
        {
            this.configuration = configuration;
            //userCreationDAO = new UserCreationDAO(this.configuration);
            loggerService = new LoggerService(this.configuration);
            authenticationSecurity = new AuthenticationSecurity(this.configuration);
            hasher = new Hasher();
        }


        private bool IsNullString(string checkString)
        {
            return checkString.IsNullOrEmpty();
        }

        private bool IsValidLength (string checkString, int minLength, int maxLength)
        {
            int length = checkString.Length;
            return length >= minLength && length <= maxLength;
        }

        public bool IsValidDigit(string checkString, string allowedPattern)
        {
            return Regex.IsMatch(checkString, allowedPattern);
        }

        
        public string GenerateSku( string allowedSku, int allowedSkuNumb)          // allowedNumberofDigit = 12
        {
            StringBuilder result = new StringBuilder(allowedSkuNumb);
            string sku;
            do
            {
                for (int i = 0; i < allowedSkuNumb; i++)
                {
                    result.Append(allowedSku[random.Next(allowedSku.Length)]);
                }
                sku = result.ToString();
            }
            while (sku.Length > 0);
            return sku;
        }


        private bool CreateNewItem(string name, string creatorHash, string sku, string price, string desc, int stock,
            string cost, bool offer, string sellerContact, List<string> image, List<string> video, DateTime dateCreated )
        {
            if ( IsNullString(name) || !IsValidLength(name, 5, 250) || !IsValidDigit(name, @"^[a-zA-Z0-9@. -]*$")) 
            {
                return false;
            }
            if (IsNullString(creatorHash) || !IsValidLength(creatorHash, 5, 64) || !IsValidDigit(creatorHash, @"^[a-zA-Z0-9@]"))
            {
                return false;
            }
            if (IsNullString(desc) || !IsValidLength(desc, 1, 3000) || !IsValidDigit(desc, @"^[a-zA-Z0-9@. -]*$"))
            {
                return false;
            }
            if (IsNullString(price) || !IsValidLength(price, 4, 7) || !IsValidDigit(price, @"^[0-9@.]"))
            {
                return false;
            }
            if (stock < 0 || stock > 1000)
            {
                return false;
            }
            if (IsNullString(cost) || !IsValidLength(cost, 4, 7) || !IsValidDigit(cost, @"^[0-9@.]"))
            {
                return false;
            }
            if (IsNullString(sellerContact) || !IsValidLength(sellerContact, 1, 300) || !IsValidDigit(sellerContact, @"^[a-zA-Z0-9@. -]*$"))
            {
                return false;
            }
            
            dateCreated = DateTime.Now;




            return true;
        }




    }
}
