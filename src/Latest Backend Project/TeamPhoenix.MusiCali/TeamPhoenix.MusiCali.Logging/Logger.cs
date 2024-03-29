﻿using daoMaria = TeamPhoenix.MusiCali.DataAccessLayer.MariaDB;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;
using System.Data.SqlClient;

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
    }
}