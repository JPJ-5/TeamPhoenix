using System;
using Phoenix.MusiCali.Models;


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
	public string securityQuestion = "";
	public string hashedSecurityAnswer = "";
}

public class Recovery : IRecovery
{
	public Result recoverDisabledAccount(string username)
	{
		UserRecovery userRecovery = new UserRecovery(username, "", DateTime.Now, DateTime.Now);
		Result recoveryResult = new Result();

		string sentOTP = generateOTP(userRecovery);

		Console.WriteLine("Enter your OTP: ");
		userRecovery.OTP = Console.ReadLine();



		while(userRecovery.validCred is false and userRecovery.failedAttempts < 5)
		{
			DateTime now = DateTime.Now;
			System.TimeSpan otpElapsed = now.Subtract(userRecovery.otpTimestamp);
			System.TimeSpan maxWaitTime = new System.TimeSpan(0, 2, 0);
			if (otpElapsed > maxWaitTime)
			{
				recoveryResult.Success = false;
				recoveryResult.ErrorMessage = "Expired OTP.";
				Console.WriteLine("Your OTP has expired.");
				return recoveryResult;
			}
			else if(isValidOTP(userRecovery.OTP) is false)
			{
				userRecovery.failedAttempts++;
				Console.WriteLine("Invalid OTP, please enter a valid OTP: ");
				userRecovery.OTP = Console.ReadLine();
			}
			else
			{
				//need to call hasher, then compare with table
				if(userRecovery.OTP == sentOTP)
				{
					userRecovery.validCred = true;
				}
			}
		}

		if(userRecovery.validCred is true)
		{
			recoveryResult.Success = true;
			return recoveryResult;
		}

		else if(userRecovery.validCred is false)
		{
			recoveryResult.Success = false;
			recoveryResult.ErrorMessage = "User exceeded 5 recovery attempts.";
			Console.WriteLine("You have exceeded 5 recovery attempts.");
			//probably lock the user out for 24 hours
			return recoveryResult;
		}

		return recoveryResult;
	}

	public Result recoverSecurityQuestion(string username)
	{
		UserRecovery userRecovery = new UserRecovery(username, "", DateTime.Now, DateTime.Now);
		Result recoveryResult = new Result();
		userRecovery.securityCode = securityCode;
		//needs to fetch security question(s) from database
		Console.WriteLine(userRecovery.securityQuestion);
		userRecovery.hashedSecurityAnswer = Console.ReadLine();

		//needs to hash security answer and then compare with database

		while(userRecovery.validCred is false and userRecovery.failedAttempts < 5)
		{
			if(isValidSecurityAnswer(userRecovery.securityCode) is false)
			{
				userRecovery.failedAttempts++;
				Console.WriteLine("Invalid answer, please enter a valid code: ");
				userRecovery.hashedSecurityAnswer = Console.ReadLine();
			}
			else
			{
				//call hasher
				bool success = validateCred(username, null, userRecovery.hashedSecurityAnswer);
				if(success)
				{
					userRecovery.validCred = true;
				}
				else
				{
					userRecovery.failedAttempts++;
					Console.WriteLine("Incorrectd answer, please try again: ");
					userRecovery.hashedSecurityAnswer = Console.ReadLine();
				}
			}
		}

		if(userRecovery.validCred is true)
		{
			recoveryResult.Success = true;
			return recoveryResult;
		}
		else if(userRecovery.validCred is false)
		{
			recoveryResult.Success = false;
			recoveryResult.ErrorMessage = "User exceeded 5 MFA recovery attempts";
			Console.WriteLine("You have exceeded 5 recovery attempts.");
			return recoveryResult;
		}
		return recoveryResult;
	}

	public static string generateOTP(string username)
	{
		string OTP = "";
		var letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		var random = new Random();

		for (var i = 0; i < 8; i++)
		{
			OTP += letters[random.Next(letters.Length)].ToString();
		}

		return OTP.ToString();
		//needs to send OTP to user's MFA & store hashed OTP, not return
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
			if (username[i]) { }
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

	public bool validateCred(string username, string OTP, string securityAnswer)
	{
		if(OTP is not null)
		{
			//checks database for username and OTP
			try
			{

			}
			catch
			{
				return false;
			}
		}

		if(securityAnswer is not null)
		{
			//checks database for username and answer to security question
			try
			{

			}
			catch
			{
				return false;
			}
		}
		return true;
	}

	public string fetchUserQuestion(string username)
	{
		try
		{

		}
		catch
		{
			return "";
		}
	}

	public Result recover(string username)
	{
		try
		{

		}
		catch
		{

		}
	}
}
