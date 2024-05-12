using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.Logging;
using Microsoft.Extensions.Configuration;

namespace TeamPhoenix.MusiCali.Services;

public class RecoverUserService
{
    private RecoverUserDAO daoRecov;
    private readonly IConfiguration configuration;
    private LoggerService loggerService;
    private AuthenticationSecurity security;
    private AuthenticationDAO authenticationDAO;
    private Hasher hasher;
    public RecoverUserService(IConfiguration configuration)
    {
        this.configuration = configuration;
        daoRecov = new RecoverUserDAO(configuration);
        loggerService = new LoggerService(configuration);
        security = new AuthenticationSecurity(configuration);
        authenticationDAO = new AuthenticationDAO(configuration);
        hasher = new Hasher();
    }
    public bool SendRecoveryEmail(string username)
    {
        try
        {
            UserAccount userAcc = daoRecov.findUserAccount(username);
            if (userAcc is null)
            {
                throw new Exception($"Unable to find user, try again or contact admin");
            }
            Console.WriteLine(username);
            UserRecovery userR = daoRecov.GetUserRecovery(username);
            userR.Success = true;
            userAcc.Email = userR.backupEmail;
            UserAuthN userA = new UserAuthN(username);

            // Generate OTP for email confirmation
            string otp = hasher.GenerateOTP();
            DateTime otpTime = DateTime.Now;
            userA.Username = username;
            userA.OTP = otp;
            userA.otpTimestamp = otpTime;
            userA.IsDisabled = false;
            userA.FailedAttempts = 0;

            bool emailSent = security.SendConfirmationEmail(userAcc.Email, otp);
            if (!emailSent)
            {
                throw new InvalidOperationException("Unable to send otp to email, please try again.");
            }

            if (!authenticationDAO.updateAuthentication(userA))
            {
                throw new Exception($"Unable to reset auth");
            }

            if (!daoRecov.updateAccountRecovery(userAcc))
            {
                throw new Exception($"Unable to reset user");
            }

            if (!daoRecov.updateUserR(userR))
            {
                throw new Exception($"Unable to recover user, try again or contact admin");
            }

            string userHash = userR.Username;
            string level = "Info";
            string category = "View";
            string context = "Sent Recovery Email";
            loggerService.CreateLog(userHash, level, category, context);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error:{ex.ToString()}");
            return false;
        }
    }

    public bool newRecoveryEmail(string username, string email)
    {
        UserRecovery userR = daoRecov.GetUserRecovery(username);
        userR.backupEmail = email;
        if (!daoRecov.updateUserR(userR))
        {
            throw new Exception($"Unable to recover user, try again or contact admin");
        }
        string userHash = userR.Username;
        string level = "Info";
        string category = "View";
        string context = "Updated User Recovery Email";
        loggerService.CreateLog(userHash, level, category, context);
        return true;
    }


    public bool isValidUsername(string username)
    {
        return Regex.IsMatch(username, @"^[a-zA-Z0-9.@-]{8,}$");
    }


    public bool ValidateOTP(string givenOTP, string storedOTP)
    {
        return givenOTP == storedOTP;
    }

    public bool DisableUser(string username)
    {
        try
        {
            if (!daoRecov.checkUserName(username))
            {
                throw new Exception($"Does not find an account with the username, try again or contact admin");
            }
            if (!isValidUsername(username))
            {
                throw new Exception($"Answer does not match database, try again or contact admin.");
            }
            UserAuthN userDE = authenticationDAO.findUsernameInfo(username).userA!;
            if (!daoRecov.DisableUser(userDE))
            {
                throw new Exception($"Error updating UserAccount");
            }

            string userHash = daoRecov.GetUserHash(username);
            string level = "Info";
            string category = "View";
            string context = "Disable User";
            loggerService.CreateLog(userHash, level, category, context);
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error:{ex.ToString()}");
            return false;
        }

    }

    public bool EnableUser(string username)
    {
        try
        {
            if (!daoRecov.checkUserName(username))
            {
                throw new Exception($"Does not find an account with the username, try again or contact admin");
            }
            if (!isValidUsername(username))
            {
                throw new Exception($"Answer does not match database, try again or contact admin.");
            }
            UserAuthN userDE = authenticationDAO.findUsernameInfo(username).userA!;
            if (!daoRecov.EnableUser(userDE))
            {
                throw new Exception($"Error updating UserAccount");
            }
            string userHash = daoRecov.GetUserHash(username);
            string level = "Info";
            string category = "View";
            string context = "Enable User";
            loggerService.CreateLog(userHash, level, category, context);
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error:{ex.ToString()}");
            return false;
        }
    }
}