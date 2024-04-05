using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Models;
using _dao = TeamPhoenix.MusiCali.DataAccessLayer.ArtistCalendar;

namespace TeamPhoenix.MusiCali.Services
{
    public class ArtistCalendar
    {
        public static bool createGig(string posterUsername, string gigName, DateTime dateTimeStart, bool visibility, string location, string description, string pay)
        {
            //save created gig data to the database
            try
            {
                if (!IsValidLocation(location))
                {
                    throw new ArgumentException("Invalid location provided. Retry again or contact system administrator");
                }
                if (IsEmptyGigName(gigName))
                {
                    throw new ArgumentException("Gig name provided is empty. Retry again or contact system administrator");
                }
                if (!IsValidGigName(gigName))
                {
                    throw new ArgumentException("Invalid gig name provided. Retry again or contact system administrator");
                }
                if (!IsValidDescription(description))
                {
                    throw new ArgumentException("Invalid description provided. Retry again or contact system administrator");
                }
                if (!IsValidPay(pay))
                {
                    return false;
                    throw new ArgumentException("Invalid pay provided. Retry again or contact system administrator");
                }
                if (_dao.IsGigDateExist(posterUsername, dateTimeStart))
                {
                    throw new InvalidOperationException("The user already has a Gig during this time");
                }
                if (!_dao.CreateGig(posterUsername, gigName, dateTimeStart, visibility, description, location, pay))
                {
                    throw new Exception("Unable To Create Gig");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Gig: {ex.Message}");
                return false;
            }
            //maybe add a console line here for feedback.
            return true;
        }
        public static bool updateGig(DateTime oldDateTimeStart, string posterUsername, string gigName, DateTime newDateTimeStart, bool visibility, string location, string description, string pay)
        {
            //save created gig data to the database
            try
            {
                if (!IsValidLocation(description))
                {
                    throw new ArgumentException("Invalid location provided. Retry again or contact system administrator");
                }
                if (!IsValidDescription(description))
                {
                    throw new ArgumentException("Invalid description provided. Retry again or contact system administrator");
                }
                if (IsEmptyGigName(gigName))
                {
                    throw new ArgumentException("Gig name provided is empty. Retry again or contact system administrator");
                }
                if (!IsValidGigName(gigName))
                {
                    throw new ArgumentException("Invalid gig name provided. Retry again or contact system administrator");
                }
                if (!IsValidPay(pay))
                {
                    throw new ArgumentException("Invalid pay provided. Retry again or contact system administrator");
                }
                if (_dao.IsGigDateExist(posterUsername, newDateTimeStart) && newDateTimeStart != oldDateTimeStart)
                {
                    throw new InvalidOperationException("The user already has a Gig during this time");
                }
                if (!_dao.EditGig(oldDateTimeStart, posterUsername, gigName, newDateTimeStart, visibility, description, location, pay))
                {
                    throw new Exception("Unable To Update Gig");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Gig: {ex.Message}");
                return false;
            }
            //maybe add a console line here for feedback.
            return true;
        }
        public static bool deleteGig(string username, DateTime dateOfGig)
        {
            try
            {
                if(!_dao.DeleteGig(username, dateOfGig))
                {
                    throw new Exception("Unable To Delete Gig");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Gig: {ex.Message}");
                return false;
            }
            return true;
        }
        public static GigView? viewGig(string username, string usernameOwner, DateTime dateOfGig)
        {
            try
            {
                return _dao.ViewGig(username, usernameOwner, dateOfGig);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Gig: {ex.Message}");
                throw;
            }
        }

        public static bool updateGigVisibility(string posterUsername, bool visibility)
        {
            //save edited gig data to the database
            try
            {
                if (!_dao.ChangeGigVisibility(posterUsername, visibility))
                {
                    throw new Exception("Unable To Update Gig Visibility");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Gig: {ex.Message}");
                return false;
            }
            //maybe add a console line here for feedback.
            return true;
        }

        private static bool IsValidLocation(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z0-9@._\-!#$%`*+/=?^’{}|~\s]+$"); //note hyphen is written using \- make sure it is not to denote a character range.
        }
        private static bool IsValidDescription(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z0-9@._\-!#$%`*+/=?^’{}|~\s]+$"); //note hyphen is written using \- make sure it is not to denote a character range.
        }
        private static bool IsValidGigName(string name)
        {
            if (name.Length < 2)
            {
                return false;
            }
            return Regex.IsMatch(name, @"^[a-zA-Z0-9@._\-!#$%`*+/=?^’{}|~\s]+$"); //note hyphen is written using \- make sure it is not to denote a character range.
        }
        private static bool IsEmptyGigName(string gigName)
        {
            return string.IsNullOrWhiteSpace(gigName);
        }
        private static bool IsValidPay(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z0-9@._\-!#$%`*+/=?^’{}|~\s]+$"); //note hyphen is written using \- make sure it is not to denote a character range.
        }
    }
}
