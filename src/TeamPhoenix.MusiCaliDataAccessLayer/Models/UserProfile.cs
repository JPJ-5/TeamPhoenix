using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class UserProfile
    {

        public string Username { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DOB { get; set; }

        //public string Password{get; set;}

        public UserProfile(string userName, string firstName, string lastName, DateTime dateOfBirth)
        {

            Username = userName;
            FirstName = firstName;
            LastName = lastName;
            DOB = dateOfBirth;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Username: {Username}, Name: {FirstName} {LastName}, Date of Birth: {DOB}");
        }
    }
}
