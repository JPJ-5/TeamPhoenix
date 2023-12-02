using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.MusiCali.Models
{
    public class UserAccount
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsDisabled { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime? LastFailedAttemptTime { get; set; }
        public string OTP { get; set; }
        public bool isAuth {  get; set; }
    }
}
