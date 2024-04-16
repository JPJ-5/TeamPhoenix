using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class UserAccount
    {
        public string Username { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string UserHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public UserAccount(string username, string salt,string userHash,string email)
        {
            Username = username;
            Salt = salt;
            Email = email;
            UserHash = userHash;
        }

        public UserAccount() {}

    }
}
