using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Model;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class DeliveryAdditionalPriceDao : BaseDao
    {
        private readonly string TABLE_NAME = "delivery_additional_price";

        public List<DeliveryExtraService> Get()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                mySqlCmd = GenerateQueryCmd(TABLE_NAME);
                reader = PerformSqlQuery(mySqlCmd);

                var result = new List<DeliveryExtraService>();
                while (reader.Read())
                {
                    result.Add(new DeliveryExtraService()
                    {
                        id = reader["id"].ToString(),
                        name = reader["name"].ToString(),
                        value = reader.GetFloat("value")
                    });
                }

                return result;
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