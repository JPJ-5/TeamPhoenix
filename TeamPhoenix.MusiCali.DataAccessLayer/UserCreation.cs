using TeamPhoenix.MusiCali.DataAccessLayer.Models;
namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class UserCreation
    {
        public static bool IsUserRegistered(string email)
        {
            // Check if the user is already registered in your data store
            // Your data access logic here
            return false;
        }

        public bool SaveUser(UserAuthN newUser)
        {
            return false;
        }
    }
}