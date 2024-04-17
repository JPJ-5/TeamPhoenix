using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class LoginTokens
    {
        public string? IdToken {  get; set; }
        public string? AccToken { get; set; }
        public bool Success {  get; set; }
    }
}
