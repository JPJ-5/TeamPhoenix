using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using rc = TeamPhoenix.MusiCali.Services.RecoverUser;

namespace TestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RecoverDisabledAccount_ShouldReturnTrueForSuccessfulRecovery()
        {
            string username = "thisisjoshu";
            bool result = rc.SendRecoveryEmail(username);

            Assert.IsTrue(result);
        }
    }
}