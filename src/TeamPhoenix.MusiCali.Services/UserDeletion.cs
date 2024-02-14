﻿using System;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
// Assuming logger alias is already defined to reference the TeamPhoenix.MusiCali.Logging.Logger class

namespace TeamPhoenix.MusiCali.Services
{
    public class UserDeletion
    {
        private readonly DataAccessLayer.UserDeletion _userDeletionDao;
        private readonly TeamPhoenix.MusiCali.Logging.Logger _logger; // Add logger dependency

        public UserDeletion(DataAccessLayer.UserDeletion userDeletionDao, TeamPhoenix.MusiCali.Logging.Logger logger)
        {
            _userDeletionDao = userDeletionDao;
            _logger = logger; // Initialize logger
        }

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
                    string userHash = DataAccessLayer.UserDeletion.GetUserHash(username); // Assuming GetUserHash is now public or accessible
                    string level = "Info";
                    string category = "View";
                    string context = "Deleted User";
                    _logger.CreateLog(userHash, level, category, context); // Use the logger to create a log

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