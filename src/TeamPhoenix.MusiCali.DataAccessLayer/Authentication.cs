using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class Authentication
    {
        public static Result logAuthFailure(string error)
        {
            return null;
        }
        public static UserAccount findUsernameAccount(string username)
        {
            return null;
        }

        public static UserAuthN findUsernameAuth(string username)
        {
            return null;
        }

        public static Result DisableUsername(string username)
        {
            return null;
        }

        public static Result updateAuthentication(string username)
        {
            return null;
        }
    }
}
