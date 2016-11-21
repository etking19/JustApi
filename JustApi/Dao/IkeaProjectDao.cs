using JustApi.Utility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Dao
{
    public class IkeaProjectDao : BaseDao
    {
        private readonly string TABLE_IKEA_PROJECT = "ikea_project";
        private readonly string TABLE_STATE = "states";
        private readonly string TABLE_COUNTRY = "countries";

        public Model.IkeaProject Get(string id)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT {0}.*, {1}.id as stateId, {1}.name as stateName, {2}.id as countryId, {2}.name as countryName FROM {0} " +
                    "INNER JOIN {1} ON {1}.id={0}.state_id " +
                    "INNER JOIN {2} ON {2}.id={0}.country_id " + 
                    "WHERE {0}.id=@id;",
                    TABLE_IKEA_PROJECT, TABLE_STATE, TABLE_COUNTRY);

                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@id", id);

                reader = PerformSqlQuery(mySqlCmd);

                if (reader.Read())
                {
                    return new Model.IkeaProject()
                    {
                        id = reader["id"].ToString(),
                        name = reader["name"].ToString(),
                        address2 = reader["address_2"].ToString(),
                        address3 = reader["address_3"].ToString(),
                        postcode = reader["postcode"].ToString(),
                        gpsLatitude = reader.GetFloat("gps_latitude"),
                        gpsLongitude = reader.GetFloat("gps_longitude"),
                        state = new Model.State()
                        {
                            stateId = reader["stateId"].ToString(),
                            name = reader["stateName"].ToString(),
                            countryId = reader["country_id"].ToString()
                        },
                        country = new Model.Country()
                        {
                            countryId = reader["countryId"].ToString(),
                            name = reader["countryName"].ToString()
                        }
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