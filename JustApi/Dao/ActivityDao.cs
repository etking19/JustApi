using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class ActivityDao : BaseDao
    {
        private readonly string TABLE_ACTIVITIES = "activities";

        public List<Model.Activity> Get()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                mySqlCmd = GenerateQueryCmd(TABLE_ACTIVITIES);
                reader = PerformSqlQuery(mySqlCmd);

                List<Model.Activity> activityList = new List<Model.Activity>();
                while (reader.Read())
                {
                    activityList.Add(new Model.Activity()
                    {
                        activityId = reader["id"].ToString(),
                        name = reader["name"].ToString()
                    });
                }

                return activityList;
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