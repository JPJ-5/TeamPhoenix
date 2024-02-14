using MySqlX.XDevAPI.CRUD;
using System;
using System.Security;
using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;
using daoRecov = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser;
using System.Diagnostics.Eventing.Reader;


namespace TeamPhoenix.MusiCali.Services;

public class RecoverUser
{
    public static bool recoverDisabledAccount(string username, string answer)
    {
        try
        {
            if (!daoRecov.checkUserName(username))
            {
                throw new Exception($"Does not find an account with the username, try again or contact admin");
            }
            UserRecovery userR = daoRecov.GetUserRecovery(username);
            if (!ValidateAnswer(answer, userR.Answer))
            {
                throw new Exception($"Answer does not match database, try again or contact admin.");

            }
            userR.Success = true;
            if (!daoRecov.updateUserR(userR))
            {
                throw new Exception($"Unable to recover user, try again or contact admin");
            }

            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error:{ex.ToString()}");
            return false;
        }
    }


    public static bool isValidUsername(string username)
    {
        return Regex.IsMatch(username, @"^[a-zA-Z0-9.@-]{8,}$");
    }


    public static bool ValidateAnswer(string answer, string userAnswer)
    {
        return answer == userAnswer;
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