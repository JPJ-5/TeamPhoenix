namespace Services
{
    using Phoenix.MusiCali.Models;
    using System;
    using System.Net.Mail;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using au = Phoenix.MusiCali.DataAccessLayer.Authentication;
    using hash = Phoenix.MusiCali.Models.Hasher;

    public class Authentication
    {


        public static bool Authenticate(string username, string password, string ipAddress)
        {
            if (isValidUsername(username))
            {
                UserAuth userA = au.findUsernameAuth(username);
                if (userA != null) 
                {
                    if (userA.IsAuth is true)
                    {
                        if(ValidatePassword(userA, password)) 
                        {
                            au.updateAuthentication(username);
                            return true;
                        }
                        RecordFailedAttempt(userA, ipAddress);
                        return false;
                    }
                    else 
                    {
                        if (ValidateOTP(userA, password))
                        {
                            au.updateAuthentication(username);
                            return true;
                        }
                        return false;
                    }
                }

                Console.WriteLine($"Username not found in database, start registration process to create a username and recieve otp");
                return false;
            }

            Console.WriteLine($"invalid username entry does not meet guidelines");
            return false;
        }

        public static void RecordFailedAttempt(UserAuth userA, string ipAddress)
        {
            userA.FailedAttempts++;
            userA.LastFailedAttemptTime = DateTime.Now;
            Console.WriteLine($"Failed attempt for account {userA.Username} from IP {ipAddress}");

            if (userA.FailedAttempts >= 3)
            {
                userA.IsDisabled = true;
                Console.WriteLine($"Account {userA.Username} disabled due to too many failed attempts.");
            }
        }

        public static bool IsValidOTP(string otp)
        {
            // Check if OTP length is at least 8 characters
            if (otp.Length < 8)
            {
                return false;
            }

            // Check if OTP contains only valid characters (a-z, A-Z)
            if (!Regex.IsMatch(otp, "^[a-zA-Z]+$"))
            {
                return false;
            }

            return true;
        }

        public static bool ValidateOTP(UserAuth userA, string otp)
        {
            if (IsValidOTP(otp))
            {
                if (hash.VerifyPassword(otp, userA.OTP, userA.Salt))
                {
                    TimeSpan timePassed = DateTime.UtcNow - userA.otpTimestamp;
                    if (timePassed.TotalMinutes > 2)
                    {
                        Console.WriteLine($"OTP has expired ");
                        return false;
                    }
                    return true;
                }
                    Console.WriteLine($"OTP does not match");
                    return false;
            }
            Console.WriteLine($"OTP given not within required parameters");
            return false;
        }

        public static bool ValidatePassword(UserAuth userA, string password)
        {
            if (hash.VerifyPassword(password, userA.Password, userA.Salt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isValidUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username) && username.Length >= 6 && username.Length <= 30 && !username.Contains(" ");
        }
    }

}