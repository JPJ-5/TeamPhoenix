using daoMaria = TeamPhoenix.MusiCali.DataAccessLayer.MariaDB;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;
using System.Data.SqlClient;
using TeamPhoenix.MusiCali.DataAccessLayer;
using recoverUser = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser; // used to get userHash


namespace TeamPhoenix.MusiCali.Logging
{

    public class Logger
    {

        public static Result CreateLog(string UserHash, string logLevel, string logCategory, string context)
        {
            // calling dao function createLog
            var result = daoMaria.CreateLog(UserHash, logLevel, logCategory, context);
            return result;
        }
        public static Result LogFeature(string UserName, string Feature)
        {
            //logging
            var level = "Info";
            var category = "View";
            var context = "User is using " + Feature;
            var userHash = recoverUser.GetUserHash(UserName);
            // calling dao function createLog
            var result = CreateLog(userHash, level, category, context);
            return result;
        }
    }
}