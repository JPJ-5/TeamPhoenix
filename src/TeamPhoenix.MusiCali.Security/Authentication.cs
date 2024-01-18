using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security.Contracts;
using dao = TeamPhoenix.MusiCali.DataAccessLayer.Authentication;
using log = TeamPhoenix.MusiCali.Logging.Authentication;

namespace TeamPhoenix.MusiCali.Security
{
    public class Authentication : IAuthentication
    {

        public Principal Authenticate(string username, string password)
        {
            Principal appPrincipal = null;
            if (!IsValidUsername(username))
            {
                throw new ArgumentException("Invalid security credentials provided. Retry again or contact the system administrator");
            }

            AuthResult authR = dao.findUsernameInfo(username);
            UserAccount userAcc = authR.userAcc;
            UserAuthN userA = authR.userA;
            UserClaims userC = authR.userC;

            if (userA == null)
            {
                throw new Exception("Invalid security credentials provided. Retry again or contact the system administrator");
            }
            if (userA.IsDisabled)
            {
                throw new Exception("Account is disabled. Perform account recovery first or contact the system administrator");
            }
            try
            {
                if (userA.IsAuth)
                {
                    if (ValidatePassword(userA, password))
                    {
                        userA.FailedAttempts = 0;
                        dao.updateAuthentication(userA);
                        Dictionary<string, string> claims = userC.Claims;
                        appPrincipal = new Principal(userA.Username, claims);
                    }

                    userA = RecordFailedAttempt(userA, userAcc);
                    throw new Exception("Invalid security credentials provided. Retry again or contact the system administrator");
                }
                else
                {
                    TimeSpan timePassed = DateTime.Now - userA.otpTimestamp;
                    if (timePassed.TotalMinutes > 2)
                    {
                        throw new Exception("OTP has expired");
                    }
                    if (!IsValidOTP(password))
                    {
                        throw new Exception("Invalid OTP used. Please request a new OTP");
                    }
                    if (ValidateOTP(userA, password))
                    {
                        userA.FailedAttempts = 0;
                        dao.updateAuthentication(userA);
                        Dictionary<string, string> claims = userC.Claims;
                        appPrincipal = new Principal(userA.Username, claims);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error authnticating {ex.Message}");
            }
            return appPrincipal;

        }


        private bool ValidatePassword(UserAuthN userA, string password)
        {
            if (Hasher.VerifyPassword(password, userA.Password, userA.Salt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsValidUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username) && username.Length >= 6 && username.Length <= 30 && !username.Contains(" ");
        }

        private UserAuthN RecordFailedAttempt(UserAuthN userA, UserAccount userAcc)
        {
            if (userA.FailedAttempts == 0)
            {
                userA.FirstFailedAttemptTime = DateTime.Now;
                userA.FirstFailedAttemptTime = DateTime.Now;
            }

            Result logResult = new Result();
            Result res = new Result();

            userA.FailedAttempts++;
            logResult.ErrorMessage = $"Failed attempt for account {userA.Username}";
            logResult.Success = false;
            res.Success = false;
            res.ErrorMessage = "Invalid security credentials provided. Retry again or contact the system administrator";
            TimeSpan timeFrame = DateTime.UtcNow - userA.FirstFailedAttemptTime;

            if (userA.FailedAttempts >= 3 && timeFrame.TotalHours > 24)
            {
                userA.IsDisabled = true;
                logResult.ErrorMessage = $"Account {userA.Username} disabled due to too many failed attempts.";
                res.ErrorMessage = "Invalid security credentials provided, account has been disabled due to 3 failed attempts. Retry again or contact the system administrator";
            }

            log.logFailure(userAcc.UserHash);
            return userA;
        }

        private bool ValidateOTP(UserAuthN userA, string otp)
        {
            if (Hasher.VerifyPassword(otp, userA.OTP, userA.Salt))
            {
                return true;
            }
            else
            {
                return false;
                throw new Exception("Invalid OTP credentials provided. Retry again or contact the system administrator");
            }
        }

        private bool IsValidOTP(string otp)
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

    }
}