using System.Text.RegularExpressions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;
using Microsoft.Extensions.Configuration; //change to project reference later

namespace TeamPhoenix.MusiCali.Services
{
    public class ArtistCalendarService
    {
        private readonly IConfiguration configuration;
        private readonly ArtistCalendarDAL artistCalendarDAL;

        public ArtistCalendarService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.artistCalendarDAL = new ArtistCalendarDAL(this.configuration);
        }
        public Result CreateGigService(string posterUsername, string gigName, DateTime dateTimeStart, bool visibility, string location, string description, string pay)
        {
            Result gigCreatedResult = new Result("", false); //result should default to false.

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
                    throw new ArgumentException("Invalid pay provided. Retry again or contact system administrator");
                }
                if (artistCalendarDAL.IsGigDateExist(posterUsername, dateTimeStart))
                {
                    throw new InvalidOperationException("The user already has a Gig during this time");
                }
                if (!artistCalendarDAL.CreateGig(posterUsername, gigName, dateTimeStart, visibility, description, location, pay))
                {
                    throw new Exception("Unable To Create Gig due to a database error."); //maybe change this if it is possible to make it clear what went wrong in the database.
                }
                else
                {
                    gigCreatedResult = new Result("Gig successfully created.", true);
                }
            }
            catch (Exception ex)
            {
                gigCreatedResult = new Result($"Error creating Gig: {ex.Message}", false);
                return gigCreatedResult;
            }
            return gigCreatedResult;
        }
        public Result UpdateGigService(DateTime oldDateTimeStart, string posterUsername, string gigName, DateTime newDateTimeStart, bool visibility, string location, string description, string pay)
        {
            Result gigUpdatedResult = new Result("", false); //result should default to false.

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
                if (artistCalendarDAL.IsGigDateExist(posterUsername, newDateTimeStart) && newDateTimeStart != oldDateTimeStart)
                {
                    throw new InvalidOperationException("The user already has a Gig during this time");
                }
                if (!artistCalendarDAL.EditGig(oldDateTimeStart, posterUsername, gigName, newDateTimeStart, visibility, description, location, pay))
                {
                    throw new Exception("Unable To Update Gig due to a database error");
                }
                else
                {
                    gigUpdatedResult = new Result("Gig successfully updated.", true);
                }
            }
            catch (Exception ex)
            {
                gigUpdatedResult = new Result($"Error updating Gig: {ex.Message}", false);
                return gigUpdatedResult;
            }
            //maybe add a console line here for feedback.
            return gigUpdatedResult;
        }
        public Result DeleteGigService(string username, DateTime dateOfGig)
        {
            Result gigDeletedResult = new Result("", false); //result should default to false.
            try
            {
                if (!artistCalendarDAL.DeleteGig(username, dateOfGig))
                {
                    throw new Exception("Unable To Delete Gig");
                }
                else
                {
                    gigDeletedResult = new Result("Gig successfully created.", true);
                }

            }
            catch (Exception ex)
            {
                gigDeletedResult = new Result($"Error updating Gig: {ex.Message}", false);
                return gigDeletedResult;
            }
            return gigDeletedResult;
        }
        public GigView? ViewGigService(string username, string usernameOwner, DateTime dateOfGig)
        {
            GigView? gigViewResult = null; //result should default to false.
            try
            {
                gigViewResult = artistCalendarDAL.ViewGig(username, usernameOwner, dateOfGig); ;
                if (gigViewResult == null)
                {
                    throw new Exception("Gig could not be found matching that date and time.");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error viewing Gig: {ex.Message}";
                gigViewResult = null;
            }
            return gigViewResult;
        }

        public Result UpdateGigVisibilityService(string posterUsername, bool visibility)
        {
            Result gigVisibilityResult = new Result("", false); //result should default to false.
            //save edited gig data to the database
            try
            {
                if (!artistCalendarDAL.ChangeGigVisibility(posterUsername, visibility))
                {
                    throw new Exception("Unable To Update Gig Visibility");
                }
                gigVisibilityResult = new Result($"Visibility settings successfully updated", true);
            }
            catch (Exception ex)
            {
                gigVisibilityResult = new Result($"Error updating Gig Visibility: {ex.Message}", false);
                return gigVisibilityResult;
            }
            //maybe add a console line here for feedback.
            return gigVisibilityResult;
        }

        private bool IsValidLocation(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z0-9@._\-!#$%`*+/=?^’{}|~\s]+$"); //note hyphen is written using \- make sure it is not to denote a character range.
        }
        private bool IsValidDescription(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z0-9@._\-!#$%`*+/=?^’{}|~\s]+$"); //note hyphen is written using \- make sure it is not to denote a character range.
        }
        private bool IsValidGigName(string name)
        {
            if (name.Length < 2)
            {
                return false;
            }
            return Regex.IsMatch(name, @"^[a-zA-Z0-9@._\-!#$%`*+/=?^’{}|~\s]+$"); //note hyphen is written using \- make sure it is not to denote a character range.
        }
        private bool IsEmptyGigName(string gigName)
        {
            return string.IsNullOrWhiteSpace(gigName);
        }
        private bool IsValidPay(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z0-9@._\-!#$%`*+/=?^’{}|~\s]+$"); //note hyphen is written using \- make sure it is not to denote a character range.
        }
    }
}
