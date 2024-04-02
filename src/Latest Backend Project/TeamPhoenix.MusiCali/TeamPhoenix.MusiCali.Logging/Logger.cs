﻿using daoMaria = TeamPhoenix.MusiCali.DataAccessLayer.MariaDB;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Mysqlx.Notice.Warning.Types;
using recoverUser = TeamPhoenix.MusiCali.DataAccessLayer.RecoverUser; // used to get userHash

namespace TeamPhoenix.MusiCali.Logging
{

    public class Logger
    {

        private readonly string _connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

        public static Result CreateLog(string UserHash, string logLevel, string logCategory, string context)
        {
            // calling dao function createLog
            var result = daoMaria.CreateLog(UserHash, logLevel, logCategory, context);
            return result;
        }

        public static bool LogFeature(string UserName, string Feature)
        {
            //logging
            var level = "Info";
            var category = "View";
            var context = "User is using " + Feature;
            var userHash = recoverUser.GetUserHash(UserName);
            CreateLog(userHash, level, category, context);
            // calling dao function createLog
            var result = daoMaria.CreateLog(userHash, level, category, context);
            return !result.HasError;
        }
    }
}