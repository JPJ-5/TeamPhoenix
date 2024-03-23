using System;
using System.Collections.Generic;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class Principal
    {
        public string Username { get; set; } = string.Empty;
        public string IDToken { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public Dictionary<string, string> Claims { get; set; }

        public Principal(string username, Dictionary<string, string> claims)
        {
            Username = username;
            Claims = claims;
        }
    }
}