namespace Services
{
    using Phoenix.MusiCali.Models;
    using System;
    using System.Text.RegularExpressions;

    public class AccountCreation
    {
        public Result ValidateAccountInfo(string username, string email, string firstName, string lastName = null, DateTime dob)
        {
            Result res = new Result();
            if(IsValidUsername(username))
            {
                if(IsValidEmail(email))
                {
                    if(IsValidName(firstName))
                    {
                        if (string.IsNullOrEmpty(lastName) || IsValidName(lastName))
                        {
                            if (IsValidDOB(dob))
                            {
                                res.Success = true;
                                return res;
                            }
                            else
                            {
                                res.Success = false;
                                res.ErrorMessage = "Date of birth does not match the required format.";
                                return res;
                            }
                        }else
                        {
                            res.Success = false;
                            res.ErrorMessage = "Last name does not match the required format.";
                            return res;
                        }
                    }else
                    {
                        res.Success = false;
                        res.ErrorMessage = "First name does not match the required format.";
                        return res;
                    }
                }else
                {
                    res.Success = false;
                    res.ErrorMessage = "Email does not match the required format.";
                    return res;
                }
            }else
            {
                res.Success = false;
                res.ErrorMessage = "Username does not match the required format.";
                return res;
            }
        }

        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrEmpty(username) || username.Length < 5 || username.Length > 30)
            {
                return false;
            }

            return Regex.IsMatch(username, @"^[a-zA-Z0-9.@-]+$");
        }

        public static bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._]+@[a-zA-Z0-9._]+\.[a-zA-Z]{2,}$";
            bool isValidFormat = Regex.IsMatch(email, emailPattern);

            return isValidFormat;
        }

        public static bool IsValidName(string name)
        {
            return name.Length >= 2 && name.Length <= 25 && Regex.IsMatch(name, @"^[a-zA-Z]+$");
        }

        public static bool IsValidDOB(DateTime dob)
        {
            DateTime minDate = new DateTime(1970, 1, 1);
            DateTime maxDate = DateTime.Now;
            return dob >= minDate && dob <= maxDate;
        }
    }
}