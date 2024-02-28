using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class UserRecovery
    {
        public string Username { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool Success { get; set; }

        public UserRecovery(string username, string question, string answer)
        {
            Username = username;
            Question = question;
            Answer = answer;
            Success = false;
        }
    }
}
