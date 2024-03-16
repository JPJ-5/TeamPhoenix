using System;
using System.Data.SqlClient;
using _authDao = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser;
using _mariaDao = TeamPhoenix.MusiCali.DataAccessLayer.MariaDB;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class LogoutRepository
    {
        public async Task<bool> LogUserLogoutAsync(string userName)
        {
            try
            {
                string userInfo = _authDao.GetUserHash(userName);
                string level = "Info";
                string category = "View";
                string context = "User has Logout";
                //.CreateLog(userHash, level, category, context);

                // Directly use the logging system to log the user's logout action
                await Task.Run(() => _mariaDao.CreateLog(userInfo, level, category, context));
                return true;
                //string level = "Info";
                //string category = "View";
                //string context = "User has Logout";
                ////.CreateLog(userHash, level, category, context);

                //// Directly use the logging system to log the user's logout action
                //await Task.Run(() => _mariaDao.CreateLog(userInfo, level, category, context));

                //return true; // Return true if logging succeeded
            }
            catch (SqlException ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An SQL error occurred: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                return false; // Return false if logging failed
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false; // Return false if logging failed
            }
        }
    }
}