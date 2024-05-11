using TeamPhoenix.MusiCali.DataAccessLayer;
using Microsoft.Extensions.Configuration;

namespace TeamPhoenix.MusiCali.Services
{
    public class UserDeletionService
    {
        private UserDeletionDAO userDeletionDao;
        private readonly IConfiguration configuration;


        public UserDeletionService(IConfiguration configuration)
        {
            this.configuration = configuration;
            userDeletionDao = new UserDeletionDAO(this.configuration);
        }

        /// <summary>
        /// Deletes the user profile and account from the database.
        /// </summary>
        /// <param name="username">The username of the user to be deleted.</param>
        /// <returns>True if deletion was successful, false otherwise.</returns>
        public bool DeleteUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            try
            {
                return userDeletionDao.DeleteProfile(username);
            }
            catch (Exception ex)
            {
                // Log the exception details here, if logging is set up
                Console.WriteLine($"Error deleting user {username}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves the hash associated with a user's account.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>The hash of the user if found, otherwise an empty string.</returns>
        public string GetUserHash(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            return userDeletionDao.GetUserHash(username);
        }
    }
}