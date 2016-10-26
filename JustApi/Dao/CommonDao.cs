using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Dao
{
    public class CommonDao : BaseDao
    {
        public string Test()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                var result = "";
                string query = string.Format("SELECT * FROM {0}", "logger");

                mySqlCmd = new MySqlCommand(query);
                reader = PerformSqlQuery(mySqlCmd);
                while (reader.Read())
                {
                    result += reader["message"];
                }

                return result;
            }
            catch (Exception)
            {

            }
            finally
            {
                CleanUp(reader, mySqlCmd);
            }

            return null;
        }
    }
}