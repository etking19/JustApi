using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class SupportedAreaDao : BaseDao
    {
        private readonly string TABLE_SUPPORTED_AREA_FROM = "supported_area_from";
        private readonly string TABLE_SUPPORTED_AREA_TO = "supported_area_to";

        public string[] GetFrom()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                mySqlCmd = GenerateQueryCmd(TABLE_SUPPORTED_AREA_FROM);
                reader = PerformSqlQuery(mySqlCmd);

                List<string> fromAdds = new List<string>();
                while (reader.Read())
                {
                    fromAdds.Add(reader["name"].ToString());
                }

                return fromAdds.ToArray();
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

        public string[] GetTo()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                mySqlCmd = GenerateQueryCmd(TABLE_SUPPORTED_AREA_TO);
                reader = PerformSqlQuery(mySqlCmd);

                List<string> fromAdds = new List<string>();
                while (reader.Read())
                {
                    fromAdds.Add(reader["name"].ToString());
                }

                return fromAdds.ToArray();
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