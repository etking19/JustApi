using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Model;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class StateDao : BaseDao
    {
        private readonly string TABLE_STATE = "states";

        public List<Model.State> Get()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                mySqlCmd = GenerateQueryCmd(TABLE_STATE);
                reader = PerformSqlQuery(mySqlCmd);

                List<State> stateList = new List<State>();
                while (reader.Read())
                {
                    stateList.Add(new State()
                    {
                        stateId = reader["id"].ToString(),
                        name = reader["name"].ToString(),
                        countryId = reader["country_id"].ToString()
                    });
                }

                return stateList;
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

        public Model.State Get(string stateId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> queryParam = new Dictionary<string, string>();
                queryParam.Add("id", stateId);

                mySqlCmd = GenerateQueryCmd(TABLE_STATE, queryParam);
                reader = PerformSqlQuery(mySqlCmd);

                if (reader.Read())
                {
                    return new State()
                    {
                        stateId = reader["id"].ToString(),
                        name = reader["name"].ToString(),
                        countryId = reader["country_id"].ToString()
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


        public List<State> GetByCountryId(string countryId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> queryParam = new Dictionary<string, string>();
                queryParam.Add("country_id", countryId);

                mySqlCmd = GenerateQueryCmd(TABLE_STATE, queryParam);
                reader = PerformSqlQuery(mySqlCmd);

                List<State> stateList = new List<State>();
                while (reader.Read())
                {
                    stateList.Add(new State()
                    {
                        stateId = reader["id"].ToString(),
                        name = reader["name"].ToString(),
                        countryId = reader["country_id"].ToString()
                    });
                }

                return stateList;
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