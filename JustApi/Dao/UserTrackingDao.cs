using JustApi.Utility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Dao
{
    public class UserTrackingDao : BaseDao
    {
        private readonly string TABLE_TRACKING = "user_tracking";

        public bool Add(string userId, string latitude, string longitude)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> insertDic = new Dictionary<string, string>();
                insertDic.Add("user_id", userId);
                insertDic.Add("latitude", latitude);
                insertDic.Add("longitude", longitude);

                mySqlCmd = GenerateAddCmd(TABLE_TRACKING, insertDic);
                return (0 != PerformSqlNonQuery(mySqlCmd));
            }
            catch (Exception e)
            {
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, e.Message);
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, e.StackTrace);
            }
            finally
            {
                CleanUp(reader, mySqlCmd);
            }

            return false;
        }
    }
}