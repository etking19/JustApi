using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class DeliveryPriceDao : BaseDao
    {
        private readonly string TABLE_DELIVERY_PRICE = "delivery_price";

        public float GetPrice(string distance, string fleetTypeId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT price as price FROM {0} WHERE {1} BETWEEN min AND max and fleet_type_id={2}",
                    TABLE_DELIVERY_PRICE, distance, fleetTypeId);


                mySqlCmd = new MySqlCommand(query);
                reader = PerformSqlQuery(mySqlCmd);
                
                if (reader.Read())
                {
                    return reader.GetFloat("price");
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

            return 0;
        }
    }
}