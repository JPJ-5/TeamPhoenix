﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class UserAuthN
    {
        public string Username { get; set; }
        public string Salt { get; set; }
        public string OTP { get; set; }
        public string? Password { get; set; }
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
            Password = null;
            FirstFailedAttemptTime = DateTime.MinValue;
            FailedAttempts = 0;
            IsDisabled = false;
            IsAuth = false;
            EmailSent = false;
        }

        public UserAuthN(string username, string salt, string oTP, string? password, DateTime otpTimestamp, int failedAttempts, DateTime firstFailedAttemptTime, bool isDisabled, bool isAuth, bool emailSent)
        {
            Username = username;
            Salt = salt;
            OTP = oTP;
            Password = password;
            this.otpTimestamp = otpTimestamp;
            FailedAttempts = failedAttempts;
            FirstFailedAttemptTime = firstFailedAttemptTime;
            IsDisabled = isDisabled;
            IsAuth = isAuth;
            EmailSent = emailSent;
        }
    }
}
