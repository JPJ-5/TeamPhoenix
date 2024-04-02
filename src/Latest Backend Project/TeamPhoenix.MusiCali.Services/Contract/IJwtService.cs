using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Services.Contract
{
    public interface IJwtService
    {
        string CreateIDToken(LoginModel loginRequest);
        string CreateAccessToken(LoginModel loginRequest);
    }
}
