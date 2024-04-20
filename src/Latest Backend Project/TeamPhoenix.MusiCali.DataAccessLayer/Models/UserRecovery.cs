using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class UserRecovery
    {
        public UserRecovery() { }
        public string Username { get; set; } = string.Empty;
        public string backupEmail { get; set; } = string.Empty;
        public bool Success { get; set; }

        public UserRecovery(string username, string email)
        {
            Username = username;
            backupEmail = email;
            Success = false;
        }
    }
}
