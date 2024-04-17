using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class AccCreationModel
    {
        public string Email { get; set; } = string.Empty;
        public DateTime Dob { get; set; }
        public string Uname { get; set; } = string.Empty;
        public string Bmail { get; set; } = string.Empty;
    }
}
