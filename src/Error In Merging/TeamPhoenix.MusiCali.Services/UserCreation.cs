using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using hash = TeamPhoenix.MusiCali.Security.Hasher;
using dao = TeamPhoenix.MusiCali.DataAccessLayer.UserCreation;


namespace TeamPhoenix.MusiCali.Services
{
    public class UserCreation
    {
        private const int ConfirmationExpiryHours = 2;


        public static bool RegisterUser(string email, DateTime dateOfBirth, string username, string fname, string lname, string q, string a)
        {

            // Validate email format
            if (!IsValidEmail(email))
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

            //Validate firstname
            if(!IsValidstring(fname))
            {
                throw new ArgumentException($"Invalid Firstname provided. Retry\r\nagain or contact system administrator");
            }

            //Validate lastname
            if (!IsValidstring(lname))
            {
                throw new ArgumentException($"Invalid Lastname provided. Retry\r\nagain or contact system administrator");
            }

            //Validate answer
            if (!IsValidstring(a))
            {
                throw new ArgumentException($"Invalid answer provided. Retry\r\nagain or contact system administrator");
            }

            // Check if the user is already registered
            if (dao.IsUsernameRegistered(username))
            {
                throw new InvalidOperationException("User with this username is already registered.");
            }

            // Check if the email is already registered
            if (dao.IsEmailRegistered(email))
            {
                throw new InvalidOperationException("User with this email is already registered.");
            }

            // Generate OTP for email confirmation
            string otp = hash.GenerateOTP();
            DateTime otpTime = DateTime.Now;

            // Check if the user is already registered
            string salt = hash.GenerateSalt();
            bool check = dao.IsSaltUsed(salt);
            while(check)
            {
                salt = hash.GenerateSalt();
                check = dao.IsSaltUsed(salt);
            }
            Dictionary<string, string> claims = new Dictionary<string, string>
            {
                {"UserRole", "NormalUser"}
            };

            // Save user registration data to the database
            UserAccount userAccount = new UserAccount(username, salt, email, hash.HashPassword(username, salt));

            UserAuthN userAuth = new UserAuthN(username, otp, otpTime, salt);

            UserRecovery userR = new UserRecovery(username, q, a);

            UserClaims userC = new UserClaims(username, claims);

            UserProfile userP = new UserProfile(username, fname, lname, dateOfBirth);

            bool emailSent = SendConfirmationEmail(email, otp);
            userAuth.EmailSent = emailSent;

            try
            {
                dao.CreateUser(userAccount, userAuth, userR, userC, userP);
            }catch (Exception ex) 
            {
                throw new Exception($"Error updating UserProfile: {ex.Message}");
            }
            return true;

            Console.WriteLine($"Registration initiated. Please check your email for confirmation within {ConfirmationExpiryHours} hours.");
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
            // Your email validation logic here
            // This is a basic example; you may want to use a more sophisticated validation approach
            return Regex.IsMatch(email, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
        }

        private static bool IsValidDateOfBirth(DateTime dateOfBirth)
        {
            // Validate date of birth logic here
            // This is a basic example; adjust it based on your requirements
            DateTime minDateOfBirth = new DateTime(1970, 1, 1);
            DateTime maxDateOfBirth = DateTime.UtcNow.Date;
            return dateOfBirth >= minDateOfBirth && dateOfBirth <= maxDateOfBirth;
        }



        private static bool SendConfirmationEmail(string email, string otp)
        {
            // Your logic to send a confirmation email
            // Have not finished this as sending otp was not part of the milesotne 2
            Console.WriteLine($"Confirmation email sent to {email}. Please check your email for the OTP.");
            return false;
        }
    }
}