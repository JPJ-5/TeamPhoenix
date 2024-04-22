using MySqlX.XDevAPI.CRUD;
using System;
using System.Security;
using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;
using daoRecov = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser;
using _hash = TeamPhoenix.MusiCali.Security.Hasher;
using _uc = TeamPhoenix.MusiCali.Services.UserCreation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.IdentityModel.Tokens;


namespace TeamPhoenix.MusiCali.Services;

public class RecoverUser
{
    public static bool SendRecoveryEmail(string username)
    {
        try
        {

            UserAccount userAcc = daoRecov.findUserAccount(username);
            if (userAcc is null)
            {
                throw new Exception($"Unable to find user, try again or contact admin");
            }

            UserRecovery userR = daoRecov.GetUserRecovery(username);
            userR.Success = true;
            userAcc.Email = userR.backupEmail;
            UserAuthN userA = new UserAuthN(username);

            // Generate OTP for email confirmation
            string otp = _hash.GenerateOTP();
            DateTime otpTime = DateTime.Now;
            userA.Username = username;
            userA.OTP = otp;
            userA.otpTimestamp = otpTime;
            userA.IsDisabled = false;

            bool emailSent = _uc.SendConfirmationEmail(userAcc.Email, otp);
            if (!emailSent)
            {
                throw new InvalidOperationException("Unable to send otp to email, please try again.");
            }

            if (!daoRecov.updateAuth(userA))
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

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error:{ex.ToString()}");
            return false;
        }
    }

    public static bool newRecoveryEmail(string username, string email)
    {
        UserRecovery userR = daoRecov.GetUserRecovery(username);
        userR.backupEmail = email;
        if (!daoRecov.updateUserR(userR))
        {
            throw new Exception($"Unable to recover user, try again or contact admin");
        }

        return true;
    }


    public static bool isValidUsername(string username)
    {
        return Regex.IsMatch(username, @"^[a-zA-Z0-9.@-]{8,}$");
    }


    public static bool ValidateOTP(string givenOTP, string storedOTP)
    {
        return givenOTP == storedOTP;
    }

    public static bool DisableUser(string username)
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
            UserAuthN userDE = Authentication.findUsernameInfo(username).userA!;
            if (!daoRecov.DisableUser(userDE))
            {
                throw new Exception($"Error updating UserAccount");
            }
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error:{ex.ToString()}");
            return false;
        }

    }

    public static bool EnableUser(string username)
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
            UserAuthN userDE = Authentication.findUsernameInfo(username).userA!;
            if (!daoRecov.EnableUser(userDE))
            {
                throw new Exception($"Error updating UserAccount");
            }
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error:{ex.ToString()}");
            return false;
        }
    }

    //public  static List<string> getEmails()
    //{
    //    try
    //    {
    //        List<string> emails = daoRecov.getAllEmail();
    //        if (!emails.IsNullOrEmpty())
    //        {
    //            return emails;
    //        }
    //        throw new Exception("NullList");
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error: {ex}");
    //        return null;
    //    }
    //}
}