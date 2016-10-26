using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Model;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class CountryDao : BaseDao
    {
        private readonly string TABLE_COUNTRY = "countries";

        public Country GetCountry(string countryId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> queryParam = new Dictionary<string, string>();
                queryParam.Add("id", countryId);

                mySqlCmd = GenerateQueryCmd(TABLE_COUNTRY, queryParam);
                reader = PerformSqlQuery(mySqlCmd);
                if (reader.Read())
                {
                    return new Country()
                    {
                        countryId = reader["id"].ToString(),
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

        public List<Country> GetCountries()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                mySqlCmd = GenerateQueryCmd(TABLE_COUNTRY);
                reader = PerformSqlQuery(mySqlCmd);

                List<Country> countryList = new List<Country>();
                while (reader.Read())
                {
                    countryList.Add(new Country()
                    {
                        countryId = reader["id"].ToString(),
                        name = reader["name"].ToString()
                    });
                }

                return countryList;
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