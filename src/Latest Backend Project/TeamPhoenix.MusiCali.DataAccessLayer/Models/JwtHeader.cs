using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class JwtHeader
    {
        public string Alg { get; set; } = "HS256";
        public string Typ { get; set; } = "JWT";
    }
}
