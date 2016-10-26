using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Dao
{
    public class LoggerDao : BaseDao
    {
        private readonly string TABLE_NAME = "logger";

        public void AddLog(int type, string message)
        {
            MySqlCommand mySqlCmd = null;
            try
            {
                Dictionary<string, string> insertParam = new Dictionary<string, string>();
                insertParam.Add("type", type.ToString());
                insertParam.Add("message", message);

                mySqlCmd = GenerateAddCmd(TABLE_NAME, insertParam);
                PerformSqlNonQuery(mySqlCmd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                CleanUp(null, mySqlCmd);
            }
        }
    }
}