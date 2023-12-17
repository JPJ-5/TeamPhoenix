using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.MusiCali.Models
{
    public class UserAuthN
    {
        public string Username { get; set; }
        public string Salt { get; set; }
        public string OTP { get; set; }
        public string Password { get; set; }
        public DateTime otpTimestamp { get; set; }
        public DateTime Timestamp { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime FirstFailedAttemptTime { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsAuth {  get; set; }
        public bool IsEnabled {  get; set; }
        public bool EmailSent { get; set; }
    }

    public UserAuth(string username, string otp, string password, DateTime otpTime, DateTime time, int failed, DateTime last, bool disabled, bool auth, bool email, string salt)
    {
        
    }
}

