using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security.Contracts;
using _dao = TeamPhoenix.MusiCali.DataAccessLayer.Authentication;
using _log = TeamPhoenix.MusiCali.Logging.Authentication;
using _hash = TeamPhoenix.MusiCali.Security.Hasher;
using Microsoft.Extensions.Configuration;
using _loggerAuthN = TeamPhoenix.MusiCali.Logging.Logger;
using System.Security.Policy;
using System.Security.Claims;
using System.Text;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Google.Protobuf.WellKnownTypes;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text.Json;

namespace TeamPhoenix.MusiCali.Security
{
    public class Authentication : IAuthentication
    {
        private readonly Dictionary<string, Principal> activeSessions = new Dictionary<string, Principal>();

        private readonly IConfiguration _configuration;
        public Authentication(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool AuthenticateUsername(string username)
        {

            if (!IsValidUsername(username))
            {
                throw new ArgumentException("Invalid security credentials provided. Retry again or contact the system administrator");
            }

            AuthResult authR = _dao.findUsernameInfo(username);
            UserAccount userAcc = authR.userAcc!;
            UserAuthN userA = authR.userA!;
            UserClaims userC = authR.userC!;

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

        public bool Authenticate(string username, string otp)
        {
            AuthResult authR = _dao.findUsernameInfo(username);
            UserAccount userAcc = authR.userAcc!;
            UserAuthN userA = authR.userA!;
            UserClaims userC = authR.userC!;
            Principal appPrincipal;
            string idToken;
            string accessToken;
            Dictionary<string, string> tokens = new Dictionary<string, string>();
            
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
                        _dao.updateAuthentication(userA);
                        Dictionary<string, string> claims = userC.Claims;
                        appPrincipal = new Principal(userA.Username, claims);

                        //// If authentication is successful, create a session token
                        //idToken = GenerateIdToken(username);

                        //// Generate Access token
                        //accessToken = GenerateAccessToken(username, claims);



                        //activeSessions.Add(idToken, appPrincipal);
                        //appPrincipal.IDToken = idToken;
                        //appPrincipal.AccessToken = accessToken;
                        // Fix to return token
                        string level = "Info";
                        string category = "View";
                        string context = "User Log In";
                        _loggerAuthN.CreateLog(userAcc.UserHash, level, category, context);

                        //tokens["IdToken"] = idToken;
                        //tokens["AccessToken"] = accessToken;
                        return true;
                    }
                }
                else
                {
                    TimeSpan timePassed = DateTime.Now - userA.otpTimestamp;
                    if (timePassed.TotalHours > 2)
                    {
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
                        _dao.updateAuthentication(userA);
                        Dictionary<string, string> claims = userC.Claims;
                        appPrincipal = new Principal(userA.Username, claims);
                        //// If authentication is successful, create a session token
                        //idToken = GenerateIdToken(username);
                        //// Generate Access token
                        //accessToken = GenerateAccessToken(username, claims);
                        //activeSessions.Add(idToken, appPrincipal);
                        //appPrincipal.IDToken = idToken;
                        //appPrincipal.AccessToken = accessToken;
                        // Fix to return token
                        string level = "Info";
                        string category = "View";
                        string context = "User Log In";
                        _loggerAuthN.CreateLog(userAcc.UserHash, level, category, context);
                        //tokens["IdToken"] = idToken;
                        //tokens["AccessToken"] = accessToken;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception($"Error authenticating {ex.Message}");
                Console.WriteLine(ex.ToString());
                return false;        
            }
            // Fix to return token
            
            return false;
        }

        public Principal? GetPrincipalBySessionToken(string sessionToken)
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

            _log.logFailure(userAcc.UserHash);

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
            // Generate OTP for email confirmation
            string otp = _hash.GenerateOTP();
            DateTime otpTime = DateTime.Now;
            userA.OTP = _hash.HashPassword(otp, userA.Salt);
            userA.otpTimestamp = otpTime;
            bool emailSent = SendConfirmationEmail(userAcc.Email, otp);
            bool update = _dao.updateAuthentication(userA);
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
            string otp = _hash.GenerateOTP();
            DateTime otpTime = DateTime.Now;
            userA.OTP = _hash.HashPassword(otp, userA.Salt);
            userA.otpTimestamp = otpTime;
            bool emailSent = SendConfirmationEmail(userR.backupEmail, otp);
            bool update = _dao.updateAuthentication(userA);
            if (emailSent && update)
            {
                return true;
            }
            else
            {
                throw new InvalidOperationException("Unable to send otp to email, and update database.");
            }
        }

        //private string GenerateIdToken(string userName)
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, userName)
        //    };

        //    var token = new JwtSecurityToken(_configuration.GetSection("Jwt:Issuer").Value, _configuration.GetSection("Jwt:Audience").Value,
        //        claims, expires: DateTime.Now.AddMinutes(20), signingCredentials: credentials);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        //private string GenerateAccessToken(string userName, Dictionary<string, string> userRoles)
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var claims = new List<Claim>
        //    {
        //        // new Claim(ClaimTypes.Name, userName)      I dont want to include the username in the access token => //
        //        // Additional claims...
        //    };

        //    // Iterate over the roles in the dictionary and add them as claims
        //    foreach (var roleEntry in userRoles)
        //    {
        //        if (roleEntry.Key == "UserRole")
        //        {
        //            claims.Add(new Claim(ClaimTypes.Role, roleEntry.Value));
        //        }
        //        // Add other types of claims if needed...
        //    }

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration.GetSection("Jwt:Issuer").Value,
        //        audience: _configuration.GetSection("Jwt:Audience").Value,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(20),
        //        signingCredentials: credentials
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        public static bool SendConfirmationEmail(string email, string otp)
        {
            try
            {
                // Your email configuration
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587; // Use 587 for TLS
                string smtpUsername = "themusicali.otp@gmail.com";
                string smtpPassword = "wqpgjtdy xnsjcsvm";

                // Create a new SmtpClient with the specified configuration
                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
                smtpClient.EnableSsl = true; // Use SSL/TLS
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                // Create the email message
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(smtpUsername);
                mailMessage.To.Add(email);
                mailMessage.Subject = "MusiCali Confirmation Email";
                mailMessage.Body = $"Your OTP for MusiCali confirmation is: {otp}";

                // Send the email
                smtpClient.Send(mailMessage);

                Console.WriteLine($"Confirmation email sent to {email}. Please check your email for the OTP.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending confirmation email: {ex.Message}");
                return false;
            }
        }

        public string CreateIDJwt(LoginModel loginRequest)
        {
            // TODO: Check security credentials match Database

            var header = new JwtHeader();
            var payload = new JwtPayload()
            {
                Iss = _configuration.GetSection("Jwt:Issuer").Value!,
                Sub = loginRequest.Username,
                Aud = _configuration.GetSection("Jwt:Audience").Value!,
                Iat = DateTime.UtcNow.Ticks,
                Exp = DateTime.UtcNow.AddMinutes(20).Ticks
            };

            // TODO: Add custom permissions to payload


            var serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            var encodedHeader = Base64UrlEncode(JsonSerializer.Serialize(header, serializerOptions));
            var encodedPayload = Base64UrlEncode(JsonSerializer.Serialize(payload, serializerOptions));


            using (var hash = new HMACSHA256(Encoding.UTF8.GetBytes("simple-key")))
            {
                // String to Byte[]
                var signatureInput = $"{encodedHeader}.{encodedPayload}";
                var signatureInputBytes = Encoding.UTF8.GetBytes(signatureInput);

                // Byte[] to String
                var signatureDigestBytes = hash.ComputeHash(signatureInputBytes);
                var encodedSignature = WebEncoders.Base64UrlEncode(signatureDigestBytes);
                string jwt = $"{encodedHeader}.{encodedPayload}.{encodedSignature}";
                return jwt;
            }
        }

        //[AllowAnonymous]
        //[HttpPost("/secure/createAccessToken")]
        public string CreateAccessJwt( LoginModel loginRequest)
        {
            // TODO: Check security credentials match Database
            var info = _dao.findUsernameInfo(loginRequest.Username);
            var userRoles = info.userC!.Claims["UserRole"];
            //string claims;
            //foreach (var roleEntry in userRoles)
            //{
            //    if (roleEntry.Key == "UserRole")
            //    {
            //        claims.Add(new Claim(ClaimTypes.Role, roleEntry.Value));
            //    }
            //    // Add other types of claims if needed...
            //}

            var header = new JwtHeader();
            var payload = new JwtPayload()
            {
                Iss = _configuration.GetSection("Jwt:Issuer").Value!,
                Sub = loginRequest.Username,
                Aud = _configuration.GetSection("Jwt:Audience").Value!,
                Iat = DateTime.UtcNow.Ticks,
                Exp = DateTime.UtcNow.AddMinutes(20).Ticks,
                Azp = _configuration.GetSection("Jwt:Azp").Value!,
                Scope = userRoles.ToString()
            };

            // TODO: Add custom permissions to payload


            var serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            var encodedHeader = Base64UrlEncode(JsonSerializer.Serialize(header, serializerOptions));
            var encodedPayload = Base64UrlEncode(JsonSerializer.Serialize(payload, serializerOptions));


            using (var hash = new HMACSHA256(Encoding.UTF8.GetBytes("simple-key")))
            {
                // String to Byte[]
                var signatureInput = $"{encodedHeader}.{encodedPayload}";
                var signatureInputBytes = Encoding.UTF8.GetBytes(signatureInput);

                // Byte[] to String
                var signatureDigestBytes = hash.ComputeHash(signatureInputBytes);
                var encodedSignature = WebEncoders.Base64UrlEncode(signatureDigestBytes);
                var jwt = $"{encodedHeader}.{encodedPayload}.{encodedSignature}";
                //var jwt = new Jwt()
                //{
                //    Header = header,
                //    Payload = payload,
                //    Signature = encodedSignature
                //};
                return jwt;
            }
        }

        private static string Base64UrlEncode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);

            return WebEncoders.Base64UrlEncode(bytes);
        }
    }
}