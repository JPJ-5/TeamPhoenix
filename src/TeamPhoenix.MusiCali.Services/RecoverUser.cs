using MySqlX.XDevAPI.CRUD;
using System;
using System.Security;
using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using dao = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser;


namespace TeamPhoenix.MusiCali.Services;

public class RecoverUser
{
    public static bool recoverDisabledAccount(string username, string answer)
    {
        UserRecovery userR = dao.GetUserRecovery(username);

        if (!ValidateAnswer(answer, userR.Answer))
        {
            throw new Exception($"Answer does not match database, try again or contact admin.");
        }
        try
        {
            userR.Success = true;
            dao.updateUserR(userR);

        }catch (Exception ex)
        {
            throw new Exception($"Error updating UserProfile: {ex.Message}");
        }
        return true;
    }


    public static bool isValidUsername(string username)
    {
        return Regex.IsMatch(username, @"^[a-zA-Z0-9.@-]{8,}$");
    }


    public static bool ValidateAnswer(string answer, string userAnswer)
    {
        return answer == userAnswer;
    }
}
