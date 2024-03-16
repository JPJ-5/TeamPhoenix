using daoMaria = TeamPhoenix.MusiCali.DataAccessLayer.MariaDB;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;
using System.Data.SqlClient;

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
    }
}