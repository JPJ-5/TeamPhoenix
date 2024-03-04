using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using ModifyUserDao = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;

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

            try
            {
                ModifyUserDao.UpdateProfile(userP);
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

    }
}