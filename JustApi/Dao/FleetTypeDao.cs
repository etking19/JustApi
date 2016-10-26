using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class FleetTypeDao : BaseDao
    {
        private readonly string TABLE_FLEET_TYPES = "fleet_types";

        public List<Model.FleetType> Get()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                mySqlCmd = GenerateQueryCmd(TABLE_FLEET_TYPES);
                reader = PerformSqlQuery(mySqlCmd);
                List<Model.FleetType> fleetTypeList = new List<Model.FleetType>();
                while (reader.Read())
                {
                    fleetTypeList.Add(constructObj(reader));
                }

                return fleetTypeList;
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

        public Model.FleetType Get(string id)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> queryParam = new Dictionary<string, string>();
                queryParam.Add("id", id);

                mySqlCmd = GenerateQueryCmd(TABLE_FLEET_TYPES, queryParam);
                reader = PerformSqlQuery(mySqlCmd);
                if (reader.Read())
                {
                    return constructObj(reader);
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

        private Model.FleetType constructObj(MySqlDataReader reader)
        {
            return new Model.FleetType()
            {
                fleetTypeId = reader["id"].ToString(),
                capacity = (int)reader["capacity"],
                name = reader["name"].ToString(),
                type = reader["type"].ToString()
            };
        }
    }
}