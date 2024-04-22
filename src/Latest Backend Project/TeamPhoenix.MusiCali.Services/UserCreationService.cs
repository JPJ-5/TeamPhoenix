using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using TeamPhoenix.MusiCali.Security;
using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Services
{
    public class UserCreationService
    {
        private UserCreationDAO userCreationDAO;
        private readonly IConfiguration configuration;
        private const int ConfirmationExpiryHours = 2;
        private LoggerService loggerService;
        private AuthenticationSecurity authenticationSecurity;
        private Hasher hasher;

        public UserCreationService(IConfiguration configuration)
        {
            this.configuration = configuration;
            userCreationDAO = new UserCreationDAO(this.configuration);
            loggerService = new LoggerService(this.configuration);
            authenticationSecurity = new AuthenticationSecurity(this.configuration);
            hasher = new Hasher();
        }

        private bool RegisterUser(string email, DateTime dateOfBirth, string username, string backupEmail, string role)
        {
            Console.WriteLine(backupEmail);
            // Validate email format
            if (!IsValidEmail(email))
            {
                Console.WriteLine("Here 1");
                return false;
                throw new ArgumentException("Invalid email provided. Retry again or contact system administrator");
            }

            if (!IsValidEmail(backupEmail))
            {
                Console.WriteLine("Here 2");
                return false;
                throw new ArgumentException("Invalid email provided. Retry again or contact system administrator");
            }

            // Validate date of birth
            if (!IsValidDateOfBirth(dateOfBirth))
            {
                throw new ArgumentException($"Invalid date of birth provided. Must be within 1/1/1970 to {DateTime.UtcNow.ToShortDateString()}.");
            }
            if (!IsValidUsername(username))
            {
                throw new ArgumentException($"Invalid email provided. Retry\r\nagain or contact system administrator");
            }

            // Check if the user is already registered
            if (userCreationDAO.IsUsernameRegistered(username))
            {
                throw new InvalidOperationException("User with this username is already registered.");
            }

            // Check if the email is already registered
            if (userCreationDAO.IsEmailRegistered(email))
            {
                throw new InvalidOperationException("User with this email is already registered.");
            }

            if (userCreationDAO.IsEmailRegistered(backupEmail))
            {
                throw new InvalidOperationException("User with this email is already registered.");
            }

            // Generate OTP for email confirmation
            string otp = hasher.GenerateOTP();
            DateTime otpTime = DateTime.Now;

            // Check if the user is already registered
            string salt = hasher.GenerateSalt();
            bool check = userCreationDAO.IsSaltUsed(salt);
            while (check)
            {
                salt = hasher.GenerateSalt();
                check = userCreationDAO.IsSaltUsed(salt);
            }
            Dictionary<string, string> claims = new Dictionary<string, string>
            {
                {"UserRole", role}
            };

            // Save user registration data to the database
            UserAccount userAccount = new UserAccount(username, salt, hasher.HashPassword(username, salt), email);

            UserAuthN userAuth = new UserAuthN(username, hasher.HashPassword(otp, salt), otpTime, salt);

            UserRecovery userR = new UserRecovery(username, backupEmail);

            UserClaims userC = new UserClaims(username, claims);

            UserProfile userP = new UserProfile(username, dateOfBirth);

            bool emailSent = authenticationSecurity.SendConfirmationEmail(email, otp);
            if (!emailSent)
            {
                throw new InvalidOperationException("Unable to send otp to email, please try again.");
            }
            userAuth.EmailSent = emailSent;

            try
            {
                if (!userCreationDAO.CreateUser(userAccount, userAuth, userR, userC, userP))
                {
                    throw new Exception("Unable To Create User");
                }
                //_loggerCreation loggerCreation = new _loggerCreation();
                string level = "Info";
                string category = "View";
                string context = "Creating new user";
                loggerService.CreateLog(userAccount.UserHash, level, category, context);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Here 3");
                Console.WriteLine($"Error updating UserProfile: {ex.Message}");
                return false;
            }
            Console.WriteLine($"Registration initiated. Please check your email for confirmation within {ConfirmationExpiryHours} hours.");

            return true;
        }

        public bool RegisterNormalUser(string email, DateTime dateOfBirth, string username, string backupEmail)
        {
            try
            {
                string role = "NormalUser";
                if (!RegisterUser(email, dateOfBirth, username, backupEmail, role))
                {
                    throw new Exception("Error creating normal user");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating UserProfile: {ex.Message}");
                return false;
            }
        }

        public bool RegisterAdminUser(string email, DateTime dateOfBirth, string username, string backupEmail)
        {
            try
            {
                string role = "AdminUser";
                if (!RegisterUser(email, dateOfBirth, username, backupEmail, role))
                {
                    throw new Exception("Error creating admin user");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating UserProfile: {ex.Message}");
                return false;
            }
        }
        private bool IsValidUsername(string username)
        {
            return Regex.IsMatch(username, @"^[a-zA-Z0-9.@-]{8,}$");
        }

        private bool IsValidstring(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z]+$");
        }

        private bool IsValidEmail(string email)
        {
            //// Your email validation logic here
            //// This is a basic example; you may want to use a more sophisticated validation approach
            //return Regex.IsMatch(email, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
            return (!IsNullString(email) && IsValidLength(email, 8, 64) && IsValidDigit(email, @"^[a-zA-Z0-9@.-]*$") && IsValidPosition(email));
        }
        private bool IsNullString(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        private bool IsValidLength(string email, int minLength, int maxLength)
        {
            if (IsNullString(email))
            {
                return false; // null string is considered invalid
            }

            int length = email.Length;
            return length >= minLength && length <= maxLength;
        }

        private bool IsValidDigit(string email, string allowedEmailPattern)
        {
            if (IsNullString(email))
            {
                return false; // Invalid if the email is null or empty
            }

            return Regex.IsMatch(email, allowedEmailPattern);
        }

        private bool IsValidPosition(string email)
        {
            if (IsNullString(email))
            {
                return false; // Invalid if the email is null or empty
            }

            int atIndex = email.IndexOf('@');

            // Check if '@' is not at the start, not at the end, and occurs only once
            return atIndex > 0 && atIndex < email.Length - 1 && email.LastIndexOf('@') == atIndex;
        }


        private bool IsValidDateOfBirth(DateTime dateOfBirth)
        {
            // Validate date of birth logic here
            // This is a basic example; adjust it based on your requirements
            DateTime minDateOfBirth = new DateTime(1970, 1, 1);
            DateTime maxDateOfBirth = DateTime.UtcNow.Date;
            return dateOfBirth >= minDateOfBirth && dateOfBirth <= maxDateOfBirth;
        }

        private bool IsValidSecureAnswer(string answer)
        {
            // allowedLength = 50
            // MinLength = 3 if we use email as username
            // allowedPattern  should be = @"^[a-zA-Z0-9@. -]*$"
            return (!IsNullString(answer) && IsValidLength(answer, 3, 50) && IsValidDigit(answer, @"^[a-zA-Z0-9@. -]*$"));
        }
    }
}