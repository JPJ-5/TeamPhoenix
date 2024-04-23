using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class Jwt
    {
        public JwtHeader Header { get; set; } = new JwtHeader();
        public JwtPayload Payload { get; set; } = new JwtPayload();

        public string? Signature { get; set; } = String.Empty;
    }
}
