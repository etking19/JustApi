using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class JobDeliveryDeclinedDao : BaseDao
    {
        private readonly string TABLE_DECLINED = "job_delivery_declined";

        public bool Add(string jobId, string companyId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> addParams = new Dictionary<string, string>();
                addParams.Add("job_id", jobId);
                addParams.Add("company_id", companyId);

                mySqlCmd = GenerateAddCmd(TABLE_DECLINED, addParams);
                return (0 != PerformSqlNonQuery(mySqlCmd));
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

            return false;
        }

        public bool Remove(string jobId, string companyId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> removeParams = new Dictionary<string, string>();
                removeParams.Add("job_id", jobId);
                removeParams.Add("company_id", companyId);

                mySqlCmd = GenerateRemoveCmd(TABLE_DECLINED, removeParams);
                return (0 != PerformSqlNonQuery(mySqlCmd));
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

            return false;
        }

        public List<string> GetDeclineCompanies(string jobId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> removeParams = new Dictionary<string, string>();
                removeParams.Add("job_id", jobId);

                mySqlCmd = GenerateQueryCmd(TABLE_DECLINED, removeParams);
                reader = PerformSqlQuery(mySqlCmd);

                List<string> companyList = new List<string>();
                while (reader.Read())
                {
                    companyList.Add(reader["company_id"].ToString());
                }

                return companyList;
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