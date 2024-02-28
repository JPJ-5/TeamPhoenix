using MySqlX.XDevAPI.CRUD;
using System;
using System.Security;
using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;
using daoRecov = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser;


namespace TeamPhoenix.MusiCali.Services;

public class RecoverUser
{
    public static bool recoverDisabledAccount(string username, string givenOtp)
    {
        try
        {
            if (!daoRecov.checkUserName(username))
            {
                throw new Exception($"Does not find an account with the username, try again or contact admin");
            }
            string storedOTP = daoRecov.GetOTP(username);
            if (!ValidateOTP(givenOtp, storedOTP))
            {
                throw new Exception($"Answer does not match database, try again or contact admin.");

            }
            UserRecovery userR = daoRecov.GetUserRecovery(username);
            userR.Success = true;
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
            UserAuthN userDE = Authentication.findUsernameInfo(username).userA;
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
            UserAuthN userDE = Authentication.findUsernameInfo(username).userA;
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
}