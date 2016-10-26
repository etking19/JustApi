using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class PickUpErrorDao : BaseDao
    {
        private readonly string TABLE_PICKUP_ERR= "pick_up_error";
        public List<Model.PickupError> Get()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                mySqlCmd = GenerateQueryCmd(TABLE_PICKUP_ERR);
                reader = PerformSqlQuery(mySqlCmd);

                List<Model.PickupError> errList = new List<Model.PickupError>();
                while (reader.Read())
                {
                    errList.Add(new Model.PickupError()
                    {
                        pickupErrorId = reader["id"].ToString(),
                        name = reader["name"].ToString()
                    });
                }

                return errList;
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

        public Model.PickupError Get(string id)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> queryParam = new Dictionary<string, string>();
                queryParam.Add("id", id);

                mySqlCmd = GenerateQueryCmd(TABLE_PICKUP_ERR, queryParam);
                reader = PerformSqlQuery(mySqlCmd);
                if (reader.Read())
                {
                    return new Model.PickupError()
                    {
                        pickupErrorId = reader["id"].ToString(),
                        name = reader["name"].ToString()
                    };
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