using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class JwtPayload
    {
        public string Iss { get; set; } = string.Empty;
        public string Sub { get; set; } = string.Empty;
        public string Aud { get; set; } = string.Empty;
        public long Exp { get; set; }
        public long Iat { get; set; }


        public long? Nbf { get; set; }

        //For Access Token
        public string? Azp { get; set; } = string.Empty;

        public string? Scope { get; set; } = String.Empty;

        public ICollection<Claim> Permissions { get; set; } = Array.Empty<Claim>();
    }
}
