using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uc = Phoenix.MusiCali.DataAccessLayer.UserCreation;
using Phoenix.MusiCali.Models;
using System.Collections.Generic;

namespace Phoenix.MusiCali.Services
{
    using System;
    using hash = Hasher;
    using auth = IAuthentication;
    using System.Text.RegularExpressions;
    using global::Phoenix.MusiCali.Models;
    using static System.Net.WebRequestMethods;

    namespace Phoenix.MusiCali.Services
    {
        public class UserCreation
        {
            private const int ConfirmationExpiryHours = 2;

            public void RegisterUser(string email, DateTime dateOfBirth, string username)
            {
                ValidateInput(email, dateOfBirth);

                // Check if the user is already registered
                if (IsUserRegistered(email))
                {
                    throw new InvalidOperationException("User with this email is already registered.");
                }

                // Generate a unique username for the user
                string username = GenerateUniqueUsername(email);

                // Generate OTP for email confirmation
                string otp = auth.GenerateOTP();

                string salt = hash.GenerateSalt();

                // Save user registration data to the database
                var userAccount = new UserAccount
                {
                    Username = username,
                    Email = email,
                    Salt = salt
                };

                var userAuth = new UserAuth
                {
                    Salt = salt;
                    OTP = otp;
                }

                // Send confirmation email (assuming you have an email service)
                SendConfirmationEmail(email, otp);

                Console.WriteLine($"Registration initiated. Please check your email for confirmation within {ConfirmationExpiryHours} hours.");
            }

            private void ValidateInput(string email, DateTime dateOfBirth, string username)
            {
                // Validate email format
                if (!IsValidEmail(email))
                {
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


            private void SendConfirmationEmail(string email, string otp)
            {
                // Your logic to send a confirmation email
                // This is a simplified example; you may want to use a dedicated email service
                Console.WriteLine($"Confirmation email sent to {email}. Please check your email for the OTP.");
            }
        }
    }
}
