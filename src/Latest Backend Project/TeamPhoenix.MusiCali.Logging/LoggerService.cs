using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;
using Microsoft.Extensions.Configuration; // used to get userHash


namespace TeamPhoenix.MusiCali.Logging
{

    public class LoggerService
    {
        private readonly IConfiguration configuration;
        private MariaDBDAO mariaDBDAO;
        private RecoverUserDAO recoverUserDAO;

        public LoggerService(IConfiguration configuration)
        {
            this.configuration = configuration;
            mariaDBDAO = new MariaDBDAO(configuration);
            recoverUserDAO = new RecoverUserDAO(configuration);
        }

        public Result CreateLog(string UserHash, string logLevel, string logCategory, string context)
        {
            // calling dao function createLog
            var result = mariaDBDAO.CreateLog(UserHash, logLevel, logCategory, context);
            return result;
        }

        public Result LogFeature(string UserName, string Feature)
        {
            //logging
            var level = "Info";
            var category = "View";
            var context = "User is using " + Feature;
            var userHash = recoverUserDAO.GetUserHash(UserName);
            // calling dao function createLog
            var result = CreateLog(userHash, level, category, context);
            return result;
        }
    }
}