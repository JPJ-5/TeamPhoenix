using System;

namespace UserRecovery;

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
