using System;

namespace Phoenix.MusiCali.UserRecovery;

public class UserRecovery
{
	public string Username {  get; set; }
	public string OTP { get; set; }
	public DateTime otpTimestamp { get; set; }
	public DateTime Timestamp { get; set; }
	public short failedAttempts = 0;
	public DateTime lastFailedAttempts {  get; set; }
	public bool validCred = false;
}

public class Recovery : IRecovery
{
	public bool recover(Username, OTP)
	{
		return true;
	}

	public bool isValidUsername(string username) 
	{  
		return true;
	}

	public bool isValidOTP(string OTP)
	{
		return true;
	}

	public bool validateCred(string username, string OTP)
	{
		return true;
	}
}
