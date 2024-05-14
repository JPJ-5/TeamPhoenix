using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Security.Contracts
{
    public interface IAuthentication
    {
        public bool Authenticate(string username, string otp, string ip);
    }
}
