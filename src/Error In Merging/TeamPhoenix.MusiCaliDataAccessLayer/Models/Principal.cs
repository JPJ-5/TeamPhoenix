using System;
using System.Collections.Generic;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class Principal
    {
        public string Username { get; set; }
        public string SessionToken { get; set; }
        public Dictionary<string, string> Claims { get; set; }

        public Principal(string username, Dictionary<string, string> claims)
        {
            Username = username;
            Claims = claims;
        }
    }
}
