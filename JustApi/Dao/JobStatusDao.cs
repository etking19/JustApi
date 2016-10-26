using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class JobStatusDao : BaseDao
    {
        private readonly string TABLE_NAME = "job_status";

        public List<Model.JobStatus> Get()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                mySqlCmd = GenerateQueryCmd(TABLE_NAME);
                reader = PerformSqlQuery(mySqlCmd);
                List<Model.JobStatus> jobStatusList = new List<Model.JobStatus>();
                while (reader.Read())
                {
                    jobStatusList.Add(new Model.JobStatus()
                    {
                        jobStatusId = reader["id"].ToString(),
                        name = (string)reader["name"]
                    });
                }

                return jobStatusList;
            }
            catch(Exception e)
            {
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Error, e.Message);
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, e.StackTrace);
            }
            finally
            {
                CleanUp(reader, mySqlCmd);
            }

            return null;
        }

        public Model.JobStatus GetById(string jobStatusId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> queryParam = new Dictionary<string, string>();
                queryParam.Add("id", jobStatusId);

                mySqlCmd = GenerateQueryCmd(TABLE_NAME, queryParam);
                reader = PerformSqlQuery(mySqlCmd);
                if (reader.Read())
                {
                    return new Model.JobStatus()
                    {
                        jobStatusId = reader["id"].ToString(),
                        name = (string)reader["name"]
                    };
                }
            }
            catch (Exception e)
            {
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Error, e.Message);
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