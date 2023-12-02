namespace Services
{
    using Phoenix.MusiCali.Models;
    using System;
    using System.Net.Mail;
    using System.Security.Cryptography;
    using System.Text;
    using au = Phoenix.MusiCali.DataAccessLayer.Authentication;

    public class Authentication
    {

        public static UserAuth Authenticate(string username, string otp, string ipAddress)
        {
            var userAccount = au.isValidUsername(username);
            if (userAccount != null)
            {
                if (userAccount.IsDisabled)
                {
                    Console.WriteLine($"Account {username} is disabled. Perform account recovery or contact admin.");
                    return null;
                }

                if (ValidateOTP(userAccount, otp))
                {
                    userAccount.FailedAttempts = 0;
                    return new UserAuth { UserAccount = userAccount, OTP = otp, Timestamp = DateTime.Now };
                }

                RecordFailedAttempt(userAccount, ipAddress);
            }
            return null;
        }

        public static void RecordFailedAttempt(UserAccount userAccount, string ipAddress)
        {
            userAccount.FailedAttempts++;
            userAccount.LastFailedAttemptTime = DateTime.Now;
            Console.WriteLine($"Failed attempt for account {userAccount.Username} from IP {ipAddress}");

            if (userAccount.FailedAttempts >= 3)
            {
                userAccount.IsDisabled = true;
                Console.WriteLine($"Account {userAccount.Username} disabled due to too many failed attempts.");
            }
        }

        public static bool ValidateOTP(UserAccount userAccount, string otp)
        {
            // Implement OTP validation logic here (e.g., expiration check)
            return userAccount.OTP == otp &&
                   (DateTime.Now - userAccount.LastFailedAttemptTime)?.TotalSeconds < 86400;
        }
    }

}