using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Dao;

namespace JustApi.Utility
{
    public class DBLogger
    {
        private static DBLogger sInstance = null;
        private LoggerDao loggerDao = null;

        public enum ESeverity
        {
            Info = 0,
            Warning = 1,
            Error = 2,
            Critical = 3,
            Analytic = 99
        };

        private DBLogger()
        {
            loggerDao = new LoggerDao();
        }

        public static DBLogger GetInstance()
        {
            if(sInstance == null)
            {
                sInstance = new DBLogger();
            }

            return sInstance;
        }

        public void Log(ESeverity level, string message)
        {
            loggerDao.AddLog(Convert.ToInt32(level), message.Replace(Environment.NewLine, " "));
        }
    }
}