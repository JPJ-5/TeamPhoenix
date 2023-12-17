using System;


namespace Phoenix.MusiCali.UserRecovery;

public class UserRecovery
{
	public string Username {  get; set; }
	public string OTP { get; set; }
	public DateTime otpTimestamp { get; set; }
	public DateTime Timestamp { get; set; }
	public int failedAttempts = 0;
	public DateTime lastFailedAttempts {  get; set; }
	public bool validCred = false;
}

public class Recovery : IRecovery
{
	public Result recover(string username)
	{
		UserRecovery userRecovery = new UserRecovery(username, "", DateTime.Now, DateTime.Now);
		return new Result;
	}

	public static string generateOTP()
	{
		string OTP = "";
		var letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		var random = new Random();

		for (var i = 0; i < 8; i++)
		{
			OTP += letters[random.Next(letters.Length)].ToString();
		}

		return OTP.ToString();
	}

	public bool isValidUsername(string username) 
	{
		//compares given string to username rules
		//Minimum of 8 characters
		//a - z(case insensitive)
		//0 - 9
		//Allow the following special characters: . – @
		if(username.Length < 8 | username.Length > 30) {  return false; }
		//core reqs doesn't mention a max length for username or OTP, weirdly enough
		for(var i=0; i < username.Length; i++)
		{
			if (username[i] ==)
		}
		//see if i can just call the same method from auth
		return true;
	}

	public bool isValidOTP(string OTP)
	{
		//expires within 2 minutes
		//minimum eight characters, a-z, A-Z, 0-9
		return true;
	}

	public bool validateCred(string username, string OTP)
	{
		
		return true;
	}
}
