using System;
using Phoenix.MusiCali.Models;

namespace Phoenix.MusiCali.UserRecovery;

public interface IRecovery
{
    public Result recover(string username, string OTP);

    public bool isValidUsername(string username);

    public bool isValidOTP(string OTP);

    public bool validateCred(string username, string OTP);
}
