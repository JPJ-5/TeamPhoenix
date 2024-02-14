using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using dao = TeamPhoenix.MusiCali.DataAccessLayer.UserDeletion;
using log = TeamPhoenix.MusiCali.Logging.Logger;

namespace TeamPhoenix.MusiCali.Services
{
    public class UserDeletion
    {
        public static Result DeleteAccount(UserAccount userA)
        {
            Result result = dao.DeleteUser(userA);
            return result;
        }
    }
}