using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Tests.Models
{
    [TestClass]
    public class UserAuthTest
    {
        [TestMethod]
        public void UserAuthTest_ShouldReturnTrueForSuccessfulModel()
        {
            // Arrange
            string username = "TestUser";
            string salt = "TestSalt";
            string OTP = "TestOTP";
            DateTime OTPTime = DateTime.UtcNow;

            //Act
            UserAuthN userAuth = new UserAuthN(username, OTP, OTPTime, salt);

            // Assert
            Assert.IsNotNull(userAuth);
            Assert.AreEqual(username, userAuth.Username);
            Assert.AreEqual(salt, userAuth.Salt);
            Assert.AreEqual(OTP, userAuth.OTP);
            Assert.AreEqual(OTPTime, userAuth.otpTimestamp);
        }
    }
}
