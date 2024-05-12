using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Services
{
    public class ModifyUserService
    {
        private ModifyUserDAO modifyUserDao;
        private readonly IConfiguration configuration;

        public ModifyUserService(IConfiguration configuration)
        {
            this.configuration = configuration;
            modifyUserDao = new ModifyUserDAO(configuration);
        }
        public bool modifyProfile(UserProfile userP)
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
                modifyUserDao.UpdateProfile(userP);
            }

            catch (Exception ex)
            {
                throw new Exception($"Error updating UserProfile: {ex.Message}");
            }

            return true;
        }

        public bool isNameValid(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z]+$");
        }

    }
}