//using TeamPhoenix.MusiCali.DataAccessLayer.Models;
//using System.Collections;
//using _authnDao = TeamPhoenix.MusiCali.DataAccessLayer.Authentication;

//namespace TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.Security
//{
//    public class OtpService
//    {
//        public List<Object> GenerateNewOTP(string username)
//        {
//            try
//            {
//                List<object> otpInfo = new List<object>();
//                AuthResult authResult = _authnDao.findUsernameInfo(username);
//                DateTime currentTime = DateTime.Now;
//                if (currentTime >= authResult.userA.otpTimestamp.AddMinutes(2))
//                {
//                    authResult.userA.OTP = "";
//                    authResult.userA.otpTimestamp = currentTime;
//                }
//                else
//                {
//                    otpInfo.Add(authResult.userA.Username);
//                    otpInfo.Add(authResult.userA.OTP);
//                    otpInfo.Add(authResult.userA.otpTimestamp);
//                    return otpInfo;
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error: {ex}");
//                return null;
//            }
//        }
//    }
//}
