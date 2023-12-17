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
        public string Salt { get; set; }
        public string Email { get; set; }

    }
}
