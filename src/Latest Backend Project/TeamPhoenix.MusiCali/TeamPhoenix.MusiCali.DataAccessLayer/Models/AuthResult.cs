using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class AuthResult
    {
        public UserAuthN? userA { get; set; }
        public UserAccount? userAcc { get; set; }    

        public UserClaims? userC {  get; set; }

        public AuthResult() { }

        public AuthResult(UserAuthN userA, UserAccount userAcc, UserClaims userC)
        {
            this.userA = userA;
            this.userAcc = userAcc;
            this.userC = userC;
        }
    }
}
