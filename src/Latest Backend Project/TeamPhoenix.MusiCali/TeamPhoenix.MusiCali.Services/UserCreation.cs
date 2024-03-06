using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using _hash = TeamPhoenix.MusiCali.Security.Hasher;
using _dao = TeamPhoenix.MusiCali.DataAccessLayer.UserCreation;
using System;
using System.Net;
using System.Net.Mail;



namespace TeamPhoenix.MusiCali.Services
{
    public class UserCreation
    {
        private const int ConfirmationExpiryHours = 2;


        private static bool RegisterUser(string email, DateTime dateOfBirth, string username, string backupEmail, string role)
        {

            // Validate email format
            if (!IsValidEmail(email))
            {
                return false;
                throw new ArgumentException("Invalid email provided. Retry again or contact system administrator");
            }

            if (!IsValidEmail(backupEmail))
            {
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
            if (_dao.IsUsernameRegistered(username))
            {
                throw new InvalidOperationException("User with this username is already registered.");
            }

            // Check if the email is already registered
            if (_dao.IsEmailRegistered(email))
            {
                throw new InvalidOperationException("User with this email is already registered.");
            }

            if (_dao.IsEmailRegistered(backupEmail))
            {
                throw new InvalidOperationException("User with this email is already registered.");
            }

            // Generate OTP for email confirmation
            string otp = _hash.GenerateOTP();
            DateTime otpTime = DateTime.Now;

            // Check if the user is already registered
            string salt = _hash.GenerateSalt();
            bool check = _dao.IsSaltUsed(salt);
            while (check)
            {
                salt = _hash.GenerateSalt();
                check = _dao.IsSaltUsed(salt);
            }
            Dictionary<string, string> claims = new Dictionary<string, string>
            {
                {"UserRole", role}
            };

            // Save user registration data to the database
            UserAccount userAccount = new UserAccount(username, salt, _hash.HashPassword(username, salt), email);

            UserAuthN userAuth = new UserAuthN(username, _hash.HashPassword(otp, salt), otpTime, salt);

            UserRecovery userR = new UserRecovery(username, backupEmail);

            UserClaims userC = new UserClaims(username, claims);

            UserProfile userP = new UserProfile(username, dateOfBirth);

            bool emailSent = SendConfirmationEmail(email, otp);
            if (!emailSent)
            {
                throw new InvalidOperationException("Unable to send otp to email, please try again.");
            }
            userAuth.EmailSent = emailSent;

            try
            {
                if (!_dao.CreateUser(userAccount, userAuth, userR, userC, userP))
                {
                    throw new Exception("Unable To Create User");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating UserProfile: {ex.Message}");
                return false;
            }
            Console.WriteLine($"Registration initiated. Please check your email for confirmation within {ConfirmationExpiryHours} hours.");

            return true;
        }

        public static bool RegisterNormalUser(string email, DateTime dateOfBirth, string username, string backupEmail)
        {
            try
            {
                string role = "NormalUser";
                if (!RegisterUser(email, dateOfBirth, username,backupEmail, role))
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

        public static bool RegisterAdminUser(string email, DateTime dateOfBirth, string username, string backupEmail)
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
        private static bool IsValidUsername(string username)
        {
            return Regex.IsMatch(username, @"^[a-zA-Z0-9.@-]{8,}$");
        }

        private static bool IsValidstring(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z]+$");
        }

        private static bool IsValidEmail(string email)
        {
            //// Your email validation logic here
            //// This is a basic example; you may want to use a more sophisticated validation approach
            //return Regex.IsMatch(email, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
            return (!IsNullString(email) && IsValidLength(email, 8, 64) && IsValidDigit(email, @"^[a-zA-Z0-9@.-]*$") && IsValidPosition(email));
        }
        private static bool IsNullString(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        private static bool IsValidLength(string email, int minLength, int maxLength)
        {
            if (IsNullString(email))
            {
                return false; // null string is considered invalid
            }

            int length = email.Length;
            return length >= minLength && length <= maxLength;
        }

        private static bool IsValidDigit(string email, string allowedEmailPattern)
        {
            if (IsNullString(email))
            {
                return false; // Invalid if the email is null or empty
            }

            return Regex.IsMatch(email, allowedEmailPattern);
        }

        private static bool IsValidPosition(string email)
        {
            if (IsNullString(email))
            {
                return false; // Invalid if the email is null or empty
            }

            int atIndex = email.IndexOf('@');

            // Check if '@' is not at the start, not at the end, and occurs only once
            return atIndex > 0 && atIndex < email.Length - 1 && email.LastIndexOf('@') == atIndex;
        }


        private static bool IsValidDateOfBirth(DateTime dateOfBirth)
        {
            // Validate date of birth logic here
            // This is a basic example; adjust it based on your requirements
            DateTime minDateOfBirth = new DateTime(1970, 1, 1);
            DateTime maxDateOfBirth = DateTime.UtcNow.Date;
            return dateOfBirth >= minDateOfBirth && dateOfBirth <= maxDateOfBirth;
        }

        private static bool IsValidSecureAnswer(string answer)
        {
            // allowedLength = 50
            // MinLength = 3 if we use email as username
            // allowedPattern  should be = @"^[a-zA-Z0-9@. -]*$"
            return (!IsNullString(answer) && IsValidLength(answer, 3, 50) && IsValidDigit(answer, @"^[a-zA-Z0-9@. -]*$"));
        }

        public static bool SendConfirmationEmail(string email, string otp)
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
                mailMessage.Subject = "MusiCali Confirmation Email";
                mailMessage.Body = $"Your OTP for MusiCali confirmation is: {otp}";

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