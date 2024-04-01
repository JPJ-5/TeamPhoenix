using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class UserClaims
    {
        public string Username { get; set; }
        public Dictionary<string, string> Claims { get; set; }
        
        public UserClaims(string username, Dictionary<string, string> claims)
        {
            Username = username;
            Claims = claims;
        }
    }
}