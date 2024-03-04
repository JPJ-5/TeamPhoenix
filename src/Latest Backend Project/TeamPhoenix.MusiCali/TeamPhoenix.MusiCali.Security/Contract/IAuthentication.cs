using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Security.Contracts
{
    public interface IAuthentication
    {
        public String Authenticate(string username, string otp);
    }
}
