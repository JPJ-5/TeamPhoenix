<<<<<<< HEAD:src/Phoenix.MusiCali.Models/UserAuthN.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.MusiCali.Models
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
>>>>>>> 72f693b5df2daec32f621445a51313f102d5003c:src/Latest Backend Project/TeamPhoenix.MusiCali.DataAccessLayer/Models/UserAuthN.cs
{
    public class UserAuthN
    {
        public string Username { get; set; }
<<<<<<< HEAD:src/Phoenix.MusiCali.Models/UserAuthN.cs
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

=======
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
>>>>>>> 72f693b5df2daec32f621445a51313f102d5003c:src/Latest Backend Project/TeamPhoenix.MusiCali.DataAccessLayer/Models/UserAuthN.cs
