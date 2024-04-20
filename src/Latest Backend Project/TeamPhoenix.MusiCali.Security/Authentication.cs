using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security.Contracts;
using _dao = TeamPhoenix.MusiCali.DataAccessLayer.Authentication;
using _log = TeamPhoenix.MusiCali.Logging.Authentication;
using _hash = TeamPhoenix.MusiCali.Security.Hasher;
using Microsoft.Extensions.Configuration;
using _loggerAuthN = TeamPhoenix.MusiCali.Logging.Logger;
using System.Text;
using Newtonsoft.Json.Linq;
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
            return true;
        }

        public bool Authenticate(string username, string otp)
        {
            AuthResult authR = _dao.findUsernameInfo(username);
            UserAccount userAcc = authR.userAcc!;
            UserAuthN userA = authR.userA!;
            UserClaims userC = authR.userC!;
            Principal appPrincipal;
            
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

                        string level = "Info";
                        string category = "View";
                        string context = "User Log In";
                        _loggerAuthN.CreateLog(userAcc.UserHash, level, category, context);

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

                        string level = "Info";
                        string category = "View";
                        string context = "User Log In";
                        _loggerAuthN.CreateLog(userAcc.UserHash, level, category, context);

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

        public static bool CheckIdRoleExisting(string username, string role)
        {
            AuthResult authR = _dao.findUsernameInfo(username);
            if (authR != null && (authR.userC!.Claims["UserRole"] == role))
            {
                return true;
            }
            return false;
        }


        private static byte[] Base64UrlDecode(string input)
        {
            string output = input;
            output = output.Replace('-', '+').Replace('_', '/');
            switch (output.Length % 4)
            {
                case 0: break;
                case 2: output += "=="; break;
                case 3: output += "="; break;
                default: throw new FormatException("Illegal base64url string!");
            }
            return Convert.FromBase64String(output);
        }

        public static string getScopeFromToken(string token)
        {
            if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrEmpty(token))
            {
                var parts = token!.Split('.');
                if (parts.Length != 3)
                    return string.Empty;

                var header = parts[0];
                var payload = parts[1];
                var signature = parts[2];

                var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));

                var jObject = JObject.Parse(payloadJson);

                string scope = jObject["scope"]!.ToString();
                return scope;
            }
            return string.Empty;
        }

        public static string getUserFromToken(string token)
        {
            if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrEmpty(token))
            {
                var parts = token!.Split('.');
                if (parts.Length != 3)
                    return string.Empty;

                var header = parts[0];
                var payload = parts[1];
                var signature = parts[2];

                var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));

                var jObject = JObject.Parse(payloadJson);

                string user = jObject["sub"]!.ToString();
                return user;
            }
            return string.Empty;
        }

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
            Console.WriteLine(payload);

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

        public string CreateAccessJwt( LoginModel loginRequest)
        {
            // TODO: Check security credentials match Database
            var info = _dao.findUsernameInfo(loginRequest.Username);
            var userRoles = info.userC!.Claims["UserRole"];

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