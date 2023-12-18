using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCaliDataAccessLayer.Models
{
    public class UserAuth
    {
        public string Username { get; set; }
        public string Salt { get; set; }
        public string OTP { get; set; }
        public string? Password { get; set; }
        public DateTime otpTimestamp { get; set; }
        public DateTime? Timestamp { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime? FirstFailedAttemptTime { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsAuth { get; set; }
        public bool EmailSent { get; set; }

        public UserAuth(string username, string otp, DateTime otpTime, string salt)
        {
            Username = username;
            Salt = salt;
            OTP = otp;
            OTP = otp;
            otpTimestamp = otpTime;
            Password = null;
            Timestamp = null;
            FirstFailedAttemptTime = null;
            FailedAttempts = 0;
            IsDisabled = false;
            IsAuth = false;
            EmailSent = false;
        }
    }
}
