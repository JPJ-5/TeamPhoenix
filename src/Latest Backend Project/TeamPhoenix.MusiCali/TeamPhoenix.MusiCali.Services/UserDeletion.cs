using System;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using _logger = TeamPhoenix.MusiCali.Logging.Logger;
using _userDeletionDao = TeamPhoenix.MusiCali.DataAccessLayer.UserDeletion;

namespace TeamPhoenix.MusiCali.Services
{
    public class UserDeletion
    {
        public Result DeleteAccount(string username)
        {
            try
            {
                // Use the new DeleteProfile method
                bool success = _userDeletionDao.DeleteProfile(username);

                // Translate the boolean result to a Result object
                if (success)
                {
                    // Perform logging after successful deletion
                    string userHash = _userDeletionDao.GetUserHash(username); // Simplified reference, assuming GetUserHash is accessible
                    string level = "Info";
                    string category = "View";
                    string context = "Deleted User";
                    _logger.CreateLog(userHash, level, category, context); // Use the logger with simplified reference

                    return new Result { Success = true };
                }
                else
                {
                    return new Result { HasError = true, ErrorMessage = "Failed to delete user profile." };
                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions
                return new Result { HasError = true, ErrorMessage = ex.Message };
            }
        }
    }
}