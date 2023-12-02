using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.MusiCali.Models
{
    public class UserAuth
    {
        public UserAccount UserAccount { get; set; }
        public string OTP { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
