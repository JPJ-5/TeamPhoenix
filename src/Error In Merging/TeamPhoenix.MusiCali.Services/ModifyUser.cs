using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using dao = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;

namespace TeamPhoenix.MusiCali.Services
{
    public class ModifyUser
    {

        public static bool modifyProfile(UserProfile userP)
        {

            if (!isNameValid(userP.FirstName))
            {

                throw new InvalidDataException();
            }

            if (!isNameValid(userP.LastName))
            {

                throw new InvalidDataException();
            }

            if (!isDateOfBirth(userP.DOB))
            {

                throw new InvalidDataException();
            }

            try
            {
                dao.UpdateProfile(userP);
            }

            catch (Exception ex)
            {
                throw new Exception($"Error updating UserProfile: {ex.Message}");
            }

            return true;
        }

        public static bool isNameValid(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z]+$");
        }

        public static bool isDateOfBirth(DateTime dob)
        {
            // Validate date of birth logic here
            // This is a basic example; adjust it based on your requirements
            DateTime minDateOfBirth = new DateTime(1970, 1, 1);
            DateTime maxDateOfBirth = DateTime.UtcNow.Date;
            return dob >= minDateOfBirth && dob <= maxDateOfBirth;
        }
    }
}
