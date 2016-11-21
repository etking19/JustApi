using JustApi.Model;
using JustApi.Utility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Dao
{
    public class IkeaDao : BaseDao
    {
        private readonly string TABLE_IKEA = "ikea";
        private readonly string TABLE_PROJECT = "ikea_project";
        private readonly string TABLE_USER = "users";
        private readonly string TABLE_JOB_STATUS = "job_status";
        private readonly string TABLE_STATE = "states";
        private readonly string TABLE_COUNTRY = "countries";

        public string Add(string unitNumber, string projectId, string userId, string itemUrl)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                // add to job order status
                Dictionary<string, string> insertParam = new Dictionary<string, string>();
                insertParam.Add("unit_number", unitNumber);
                insertParam.Add("project_id", projectId);
                insertParam.Add("user_id", userId);
                insertParam.Add("item_list", itemUrl);

                mySqlCmd = GenerateAddCmd(TABLE_IKEA, insertParam);
                PerformSqlNonQuery(mySqlCmd);

                return mySqlCmd.LastInsertedId.ToString();
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

        public List<Ikea> Get(string limit, string skip)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT {0}.*, {1}.id as ikeaId, {1}.name as ikeaName, {2}.id as userId, {3}.id as jobStatusId, {3}.name as jobStatusName, {4}.id as stateId, {4}.name as stateName, {5}.id as countryId, {5}.name as countryName FROM {0} " +
                    "INNER JOIN {1} ON {1}.id={0}.project_id " +
                    "INNER JOIN {2} ON {2}.id={0}.user_id " +
                    "INNER JOIN {3} ON {3}.id={0}.job_status_id " + 
                    "INNER JOIN {4} ON {4}.id={1}.state_id " + 
                    "INNER JOIN {5} ON {5}.id={1}.country_id ",
                    TABLE_IKEA, TABLE_PROJECT, TABLE_USER, TABLE_JOB_STATUS, TABLE_STATE, TABLE_COUNTRY);

                if (limit != null)
                {
                    query += string.Format("LIMIT {0} ", limit);
                }

                if (skip != null)
                {
                    query += string.Format("OFFSET {0} ", skip);
                }

                mySqlCmd = new MySqlCommand(query);
                reader = PerformSqlQuery(mySqlCmd);

                List<Ikea> ikeaList = new List<Ikea>();
                while (reader.Read())
                {
                    ikeaList.Add(constructObj(reader));
                }

                return ikeaList;
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

        public List<Ikea> GetByProjectId(string projectId, string jobStatusId, string limit, string skip)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT {0}.*, {1}.id as ikeaId, {1}.name as ikeaName, {2}.id as userId, {3}.id as jobStatusId, {3}.name as jobStatusName, {4}.id as stateId, {4}.name as stateName, {5}.id as countryId, {5}.name as countryName FROM {0} " +
                    "INNER JOIN {1} ON {1}.id={0}.project_id " +
                    "INNER JOIN {2} ON {2}.id={0}.user_id " +
                    "INNER JOIN {3} ON {3}.id={0}.job_status_id " +
                    "INNER JOIN {4} ON {4}.id={1}.state_id " +
                    "INNER JOIN {5} ON {5}.id={1}.country_id ",
                    TABLE_IKEA, TABLE_PROJECT, TABLE_USER, TABLE_JOB_STATUS);

                if (limit != null)
                {
                    query += string.Format("LIMIT {0} ", limit);
                }

                if (skip != null)
                {
                    query += string.Format("OFFSET {0} ", skip);
                }

                if (projectId != null)
                {
                    query += string.Format("WHERE project_id=@project_id ");

                    if (jobStatusId != null)
                    {
                        query += string.Format("AND job_status_id=@jobStatusId ");
                    }
                }
                else if (jobStatusId != null)
                {
                    query += string.Format("WHERE job_status_id=@jobStatusId ");
                }


                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@project_id", projectId);
                mySqlCmd.Parameters.AddWithValue("@jobStatusId", jobStatusId);

                reader = PerformSqlQuery(mySqlCmd);

                List<Ikea> ikeaList = new List<Ikea>();
                while (reader.Read())
                {
                    ikeaList.Add(constructObj(reader));
                }

                return ikeaList;
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

        private Ikea constructObj(MySqlDataReader reader)
        {
            return new Ikea()
            {
                id = reader["id"].ToString(),
                itemUrl = reader["item_list"].ToString(),
                unitNumber = reader["unit_number"].ToString(),
                user = new User()
                {
                    userId = reader["userId"].ToString(),
                    contactNumber = reader["contact"].ToString(),
                    email = reader["email"].ToString(),
                    displayName = reader["display_name"].ToString()
                },
                project = new IkeaProject()
                {
                    id = reader["ikeaId"].ToString(),
                    name = reader["ikeaName"].ToString(),
                    address2 = reader["address_2"].ToString(),
                    address3 = reader["address_3"].ToString(),
                    postcode = reader["postcode"].ToString(),
                    gpsLatitude = reader.GetFloat("gps_latitude"),
                    gpsLongitude = reader.GetFloat("gps_longitude"),
                    state = new State()
                    {
                        stateId = reader["stateId"].ToString(),
                        name = reader["stateName"].ToString(),
                        countryId = reader["country_id"].ToString()
                    },
                    country = new Country()
                    {
                        countryId = reader["countryId"].ToString(),
                        name = reader["countryName"].ToString()
                    }
                },
                jobStatus = new JobStatus()
                {
                    jobStatusId = reader["jobStatusId"].ToString(),
                    name = reader["jobStatusName"].ToString()
                }
            };
        }
    }
}