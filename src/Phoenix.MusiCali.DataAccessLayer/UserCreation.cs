using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phoenix.MusiCali.Models;

namespace Phoenix.MusiCali.DataAccessLayer
{
    public class UserCreation
    {
        public bool IsUserRegistered(string email)
        {
            // Check if the user is already registered in your data store
            // Your data access logic here
            return false;
        }

        public bool SaveUser(UserAuth newUser)
        {
            return false;
        }
    }
}
