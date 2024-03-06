﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security.Contracts;
using dao = TeamPhoenix.MusiCali.DataAccessLayer.Authentication;
using log = TeamPhoenix.MusiCali.Logging.Authentication;
using hash = TeamPhoenix.MusiCali.Security.Hasher;
using uc = TeamPhoenix.MusiCali.Services.UserCreation;
using _loggerAuthN = TeamPhoenix.MusiCali.Logging.Logger;
using System.Security.Policy;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Configuration;

namespace TeamPhoenix.MusiCali.Security
{
    public class Authentication : IAuthentication
    {
        private readonly Dictionary<string, Principal> activeSessions = new Dictionary<string, Principal>();

        //private readonly IConfiguration _configuration;
        //public Authentication(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        //private readonly IConfiguration _configuration;
        //private IConfiguration setConfiguration(IConfiguration configuration)
        //{
        //    return configuration;
        //}


        //private IConfiguration _configuration;

        //public Authentication(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        //public IConfiguration Configuration
        //{
        //    get { return _configuration; }
        //    set { _configuration = value; }
        //}

        private readonly IConfiguration _configuration;
        public Authentication(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public bool AuthenticateUsername(string username)
        //{

        //    if (!IsValidUsername(username))
        //    {
        //        throw new ArgumentException("Invalid security credentials provided. Retry again or contact the system administrator");
        //    }

        //    AuthResult authR = dao.findUsernameInfo(username);
        //    UserAccount userAcc = authR.userAcc;
        //    UserAuthN userA = authR.userA;
        //    UserClaims userC = authR.userC;

        //    if (userA == null)
        //    {
        //        throw new Exception("Invalid security credentials provided. Retry again or contact the system administrator");
        //    }
        //    if (userA.IsDisabled)
        //    {
        //        throw new Exception("Account is disabled. Perform account recovery first or contact the system administrator");
        //    }

        //    if (userA.IsAuth == true)
        //    {
        //        bool emailSent = newOTP(userA, userAcc);
        //        if (!emailSent)
        //        {
        //            throw new InvalidOperationException("Unable to send otp to email, please try again.");
        //        }
        //    }
        //    bool emailSentAnyway = newOTP(userA, userAcc);
        //    return true;
        //}



        public bool AuthenticateUsername(string username)
        {

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

            if (userA.IsAuth)
            {
                bool emailSent = newOTP(userA, userAcc);
                if (!emailSent)
                {
                    throw new InvalidOperationException("Unable to send otp to email, please try again.");
                }
            }
            //bool emailSentAnyway = newOTP(userA, userAcc);
            return true;
        }


        public string Authenticate(string username, string otp)
        {
            AuthResult authR = dao.findUsernameInfo(username);
            UserAccount userAcc = authR.userAcc;
            UserAuthN userA = authR.userA;
            UserClaims userC = authR.userC;
            Principal appPrincipal = null;
            string idToken = null;
            try
            {
                if (userA.IsAuth)
                {
                    TimeSpan timePassed = DateTime.Now - userA.otpTimestamp;
                    if (timePassed.TotalMinutes > 2)
                    {
                        bool emailSent = newOTP(userA, userAcc);
                        if (!emailSent)
                        {
                            throw new InvalidOperationException("Unable to send otp to email, please try again.");
                        }
                    }
                    if (!IsValidOTP(otp))
                    {
                        throw new Exception("Invalid OTP used. Please request a new OTP");
                    }
                    if (ValidateOTP(userA, otp))
                    {

                        // Implement Token Part
                        userA.FailedAttempts = 0;
                        dao.updateAuthentication(userA);
                        Dictionary<string, string> claims = userC.Claims;
                        appPrincipal = new Principal(userA.Username, claims);







                        // If authentication is successful, create a session token
                        idToken = GenerateToken(username);





                        activeSessions.Add(idToken, appPrincipal);
                        appPrincipal.IDToken = idToken;
                        // Fix to return token
                        string level = "Info";
                        string category = "View";
                        string context = "User Log In";
                        _loggerAuthN.CreateLog(userAcc.UserHash, level, category, context);
                        return idToken;
                    }
                }
                else
                {
                    TimeSpan timePassed = DateTime.Now - userA.otpTimestamp;
                    if (timePassed.TotalHours > 2)
                    {
                        //bool emailSent = newOTP(userA, userAcc);
                        //if (!emailSent)
                        //{
                        //    throw new InvalidOperationException("otp expired. Unable to send otp to email, please try again.");
                        //}
                        throw new Exception("OTP has expired, New OTP has been sent to email");
                    }
                    if (!IsValidOTP(otp))
                    {
                        throw new Exception("Invalid OTP used.");
                    }
                    if (ValidateOTP(userA, otp))
                    {
                        // Implement Token Part
                        userA.FailedAttempts = 0;
                        userA.IsAuth = true;
                        dao.updateAuthentication(userA);
                        Dictionary<string, string> claims = userC.Claims;

                        appPrincipal = new Principal(userA.Username, claims);


                        // If authentication is successful, create a session token
                        idToken = GenerateToken(username);






                        activeSessions.Add(idToken, appPrincipal);
                        appPrincipal.IDToken = idToken;
                        // Fix to return token
                        string level = "Info";
                        string category = "View";
                        string context = "User Confirmed Email Verification";
                        _loggerAuthN.CreateLog(userAcc.UserHash, level, category, context);
                        return idToken;
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception($"Error authenticating {ex.Message}");
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
            // Fix to return token
            return idToken;
        }

        public Principal GetPrincipalBySessionToken(string sessionToken)
        {
            // Retrieve Principal using the session token
            if (activeSessions.TryGetValue(sessionToken, out var principal))
            {
                return principal;
            }

            return null; // Session token not found
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

        private bool newOTP(UserAuthN userA, UserAccount userAcc)
        {
            //TimeSpan timeSinceLastOTP = DateTime.Now - userA.otpTimestamp;
            //if (timeSinceLastOTP.TotalMinutes <= 2)
            //{
            //    // An OTP was recently sent, so we do not generate a new one
            //    return true; // Assuming the previous OTP email was sent successfully
            //}
            // Generate OTP for email confirmation
            string otp = hash.GenerateOTP();
            DateTime otpTime = DateTime.Now;
            userA.OTP = hash.HashPassword(otp, userA.Salt);
            userA.otpTimestamp = otpTime;
            bool emailSent = uc.SendConfirmationEmail(userAcc.Email, otp);
            bool update = dao.updateAuthentication(userA);
            if (emailSent && update)
            {
                return true;
            }
            else
            {
                throw new InvalidOperationException("Unable to send otp to email, and update database.");
            }
        }

        private bool recoveryOTP(UserAuthN userA, UserRecovery userR)
        {
            string otp = hash.GenerateOTP();
            DateTime otpTime = DateTime.Now;
            userA.OTP = hash.HashPassword(otp, userA.Salt);
            userA.otpTimestamp = otpTime;
            bool emailSent = uc.SendConfirmationEmail(userR.backupEmail, otp);
            bool update = dao.updateAuthentication(userA);
            if (emailSent && update)
            {
                return true;
            }
            else
            {
                throw new InvalidOperationException("Unable to send otp to email, and update database.");
            }
        }







        //private string GenerateToken(string UserName)
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));

        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, UserName)
        //    };

        //    var token = new JwtSecurityToken(_configuration.GetSection("Jwt:Issuer").Value, _configuration.GetSection("Jwt:Audience").Value,
        //        claims,
        //        expires: DateTime.Now.AddMinutes(20),
        //        signingCredentials: credentials);

        //    return new JwtSecurityTokenHandler().WriteToken(token);

        //}



        // Removed GetPrincipalBySessionToken and other session token related methods

        private string GenerateToken(string userName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userName)
            };

            var token = new JwtSecurityToken(_configuration.GetSection("Jwt:Issuer").Value, _configuration.GetSection("Jwt:Audience").Value,
                claims, expires: DateTime.Now.AddMinutes(20), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}