using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class UserSessionDao : BaseDao
    {
        private readonly string TABLE_SESSION = "user_session";

        public string GetLatestToken(string userId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT {0}.token FROM {0} WHERE user_id={1} AND DATE(valid_till) >= CURRENT_TIMESTAMP order by creation_date DESC LIMIT 1",
                    TABLE_SESSION, userId);

                mySqlCmd = new MySqlCommand(query);
                reader = PerformSqlQuery(mySqlCmd);
                if (reader.Read())
                {
                    return reader["token"].ToString();
                }
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

            return null;
        }
    }
}