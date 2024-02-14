using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.Models
{
    [TestClass]
    public class UserAuthNTest
    {
        [TestMethod]
        public void UserAuthNTest_ShouldReturnTrueForSuccessfulModel()
        {
            // Arrange
            string username = "TestUser";
            string salt = "TestSalt";
            string OTP = "TestOTP";
            DateTime OTPTime = DateTime.UtcNow;

            //Act
            UserAuthN userAuthN = new UserAuthN(username, OTP, OTPTime, salt);

            // Assert
            Assert.IsNotNull(userAuthN);
            Assert.AreEqual(username, userAuthN.Username);
            Assert.AreEqual(salt, userAuthN.Salt);
            Assert.AreEqual(OTP, userAuthN.OTP);
            Assert.AreEqual(OTPTime, userAuthN.otpTimestamp);
        }
    }
}
