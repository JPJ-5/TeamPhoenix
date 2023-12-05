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
        public string Password { get; set; }
        public bool isEnabled {  get; set; }
        public string Salt { get; set; }
    }
}
