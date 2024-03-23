using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class UserAuthN
    {
        public string Username { get; set; }
        public string Salt { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
        public DateTime otpTimestamp { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime FirstFailedAttemptTime { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsAuth { get; set; }
        public bool EmailSent { get; set; }

        public UserAuthN(string username, string otp, DateTime otpTime, string salt)
        {

            Username = username;
            Salt = salt;
            OTP = otp;
            otpTimestamp = otpTime;
            FirstFailedAttemptTime = DateTime.MinValue;
            FailedAttempts = 0;
            IsDisabled = false;
            IsAuth = false;
            EmailSent = false;
        }

        public UserAuthN(string username)
        {
            Username = username;
        }

        public UserAuthN(string username, string salt, string oTP, DateTime otpTimestamp, int failedAttempts, DateTime firstFailedAttemptTime, bool isDisabled, bool isAuth, bool emailSent)
        {
            Username = username;
            Salt = salt;
            OTP = oTP;
            this.otpTimestamp = otpTimestamp;
            FailedAttempts = failedAttempts;
            FirstFailedAttemptTime = firstFailedAttemptTime;
            IsDisabled = isDisabled;
            IsAuth = isAuth;
            EmailSent = emailSent;
        }
    }
}
