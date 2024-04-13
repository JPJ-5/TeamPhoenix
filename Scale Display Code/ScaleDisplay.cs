/**using _logger = TeamPhoenix.MusiCali.Logging.Logger; // Add this using directive to import the Logger class
using _uA = TeamPhoenix.MusiCali.DataAccessLayer.Authentication;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Services
{
    public class ScaleDisplay
    {
        public bool ScaleDisplayLoggingService(string username)
        {
            try
            {
                var userInfo = _uA.findUsernameInfo(username);
            
                string userHash = userInfo.userAcc.UserHash;
                string logLevel = "INFO";
                string logCategory = "ScaleDisplay";
                string context = "User Has Opened Scale Display";
                
                var result = _loggerogger.CreateLog(userHash, logLevel, logCategory, context); //calls the logger service to make the log
                return true;
            }
            catch(Excpetion Ex){
                Console.WriteLine(Ex.toString());
                return false;
            }
        }
    }
}*/
