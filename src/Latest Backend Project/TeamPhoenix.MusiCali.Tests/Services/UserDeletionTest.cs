using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uC = TeamPhoenix.MusiCali.Services.UserCreation;

namespace TeamPhoenix.MusiCali.Tests.Services
{
    [TestClass]
    public class UserDeletionTest
    {
        [TestMethod]
        public void DeleteUser_ShouldReturnTrue()
        {
            string email = "deletetest@example.com";
            string backupEmail = "backupdeleteTest@example.com";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string username = "deletetestuser";
        }
    }
}
