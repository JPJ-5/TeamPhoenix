using System.Text.RegularExpressions;

namespace TeamPhoenix.MusiCali.Services {
    public class UserCreation
    {
        private const int ConfirmationExpiryHours = 2;

        public void RegisterUser(string email, DateTime dateOfBirth, string username)
        {
            if (ValidateInput(email, dateOfBirth, username))
            {
                // Check if the user is already registered
                if (uc.IsUserRegistered(email))
                {
                    throw new InvalidOperationException("User with this email is already registered.");
                }


                // Generate OTP for email confirmation
                string otp = hash.GenerateOTP();
                DateTime otpTime = DateTime.Now;

                string salt = hash.GenerateSalt();

                // Save user registration data to the database
                var userAccount = new UserAccount(username, email, salt);

                var userAuth = new UserAuth(username, otp, otpTime, salt);

                Dictionary<string, string> claims = < "role", UserRole.User >;

                // Send confirmation email (assuming you have an email service)
                bool emailSent = SendConfirmationEmail(email, otp);
                userAuth.EmailSent = emailSent;

                Console.WriteLine($"Registration initiated. Please check your email for confirmation within {ConfirmationExpiryHours} hours.");
            }

        }

        private bool ValidateInput(string email, DateTime dateOfBirth, string username)
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
                return false;
                throw new ArgumentException($"Invalid date of birth provided. Must be within 1/1/1970 to {DateTime.UtcNow.ToShortDateString()}.");
            }
            if (!IsValidUsername(username))
            {
                return false;
                throw new ArgumentException($"Invalid email provided. Retry\r\nagain or contact system administrator");
            }
            return true;
        }

        private bool IsValidUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username) && username.Length >= 6 && username.Length <= 30 && !username.Contains(" ");
        }

        private bool IsValidEmail(string email)
        {
            // Your email validation logic here
            // This is a basic example; you may want to use a more sophisticated validation approach
            return Regex.IsMatch(email, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
        }

        private bool IsValidDateOfBirth(DateTime dateOfBirth)
        {
            // Validate date of birth logic here
            // This is a basic example; adjust it based on your requirements
            DateTime minDateOfBirth = new DateTime(1970, 1, 1);
            DateTime maxDateOfBirth = DateTime.UtcNow.Date;
            return dateOfBirth >= minDateOfBirth && dateOfBirth <= maxDateOfBirth;
        }

        private void SaveUserRegistration(UserAccount newUser)
        {
            // Your logic to save user registration data to the database
            // This is a simplified example; integrate it with your existing data access layer
            res = uc.SaveUser(newUser);

        }// Save userAuth to the database
         // Your data access logic here


        private bool SendConfirmationEmail(string email, string otp)
        {
            // Your logic to send a confirmation email
            // Have not finished this as sending otp was not part of the milesotne 2
            Console.WriteLine($"Confirmation email sent to {email}. Please check your email for the OTP.");
            return false;
        }
    }
}