using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.MusiCali.Models
{
    public class Principal
    {
        public string Username { get; set; }
        public Dictionary<string, string> Claims { get; set; }

        public Principal(string username, Dictionary<string, string> claims)
        {
            Username = username;
            Claims = claims;
        }
    }

}
