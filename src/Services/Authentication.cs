namespace Services
{
    using Phoenix.MusiCali.Models;
    using System;
    using System.Text.RegularExpressions;
    using au = Phoenix.MusiCali.DataAccessLayer.Authentication;
    using hash = Phoenix.MusiCali.Models.Hasher;

    public class Authentication
    {


        public Result Authenticate(string username, string password, string ipAddress)
        {
            Result res = new Result();
            if (isValidUsername(username))
            {
                UserAuth userA = au.findUsernameAuth(username);
                if (userA.IsDisabled == true) 
                {
                    res.Success = false;
                    res.ErrorMessage = "Account is disabled.Perform account recovery first or contact system administrator";
                    return res;
                }
                if (userA != null) 
                {
                    if (userA.IsAuth is true)
                    {
                        if(ValidatePassword(userA, password)) 
                        {
                            au.updateAuthentication(username);
                            res.Success = true;
                            return res;
                        }
                        Result res2 = RecordFailedAttempt(userA, ipAddress);
                        return res2;
                    }
                    else 
                    {
                        Result otpRes = ValidateOTP(userA, password);
                        return otpRes;
                    }
                }

                res.ErrorMessage = "Invalid security credentials provided. Retry again or contact system administrator";
                res.Success = false;
                return res;
            }

            res.ErrorMessage = "invalid username entry does not meet guidelines";
            res.Success = false;
            return res;
        }

        public static Result RecordFailedAttempt(UserAuth userA, string ipAddress)
        {
            Result res = new Result(); 
            userA.FailedAttempts++;
            userA.LastFailedAttemptTime = DateTime.Now;
            res.ErrorMessage = "Failed attempt for account {userA.Username} from IP {ipAddress}";
            res.Success = false;

            if (userA.FailedAttempts >= 3)
            {
                userA.IsDisabled = true;
                res.ErrorMessage = "Account {userA.Username} disabled due to too many failed attempts.";
            }
            return res;
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

        public Result ValidateOTP(UserAuth userA, string otp)
        {
            Result res = new Result();
            if (IsValidOTP(otp))
            {
                if (hash.VerifyPassword(otp, userA.OTP, userA.Salt))
                {
                    TimeSpan timePassed = DateTime.UtcNow - userA.otpTimestamp;
                    if (timePassed.TotalMinutes > 2)
                    {
                        res.ErrorMessage = "OTP has expired";
                        res.Success = false;
                        return res;
                    }
                    res.Success = true;
                    return res;
                }
                    res.ErrorMessage = "Invalid security credentials provided. Retry again or contact system administrator";
                    res.Success = false;
                    return res;
            }
            res.ErrorMessage = "Invalid OTP used. Please request a new OTP";
            res.Success = false;
            return res;
        }

        public bool ValidatePassword(UserAuth userA, string password)
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