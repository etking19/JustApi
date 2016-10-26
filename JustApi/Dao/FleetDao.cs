using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class FleetDao : BaseDao
    {
        private readonly string TABLE_FLEETS = "fleets";

        public string Add(Model.Fleet payload)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                DateTime dateService = DateTime.Parse(payload.serviceDueDate);
                DateTime dateRoadTax = DateTime.Parse(payload.roadTaxExpiry);

                // add to job order status
                Dictionary<string, string> insertParam = new Dictionary<string, string>();
                insertParam.Add("registration_number", payload.registrationNumber);
                insertParam.Add("fleet_types_id", payload.fleetTypeId);
                insertParam.Add("road_tax_expired", dateRoadTax.ToString("yyyy-MM-dd HH:mm:ss"));
                insertParam.Add("service_due_date", dateService.ToString("yyyy-MM-dd HH:mm:ss"));
                insertParam.Add("service_due_mileage", payload.serviceDueMileage.ToString());
                insertParam.Add("company_id", payload.companyId);
                insertParam.Add("remarks", payload.remarks);

                mySqlCmd = GenerateAddCmd(TABLE_FLEETS, insertParam);
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

        public bool Delete(string id)
        {
            MySqlCommand mySqlCmd = null;
            try
            {
                Dictionary<string, string> removeParams = new Dictionary<string, string>();
                removeParams.Add("id", id);

                mySqlCmd = GenerateSoftDelete(TABLE_FLEETS, removeParams);
                return (PerformSqlNonQuery(mySqlCmd) != 0);
            }
            catch (Exception e)
            {
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Error, e.Message);
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, e.StackTrace);
            }
            finally
            {
                CleanUp(null, mySqlCmd);
            }

            return false;
        }

        public List<Model.Fleet> Get(string limit, string skip)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                // add to job order status
                string query = string.Format("SELECT * FROM {0} WHERE deleted=0 ORDER BY creation_date DESC ",
                    TABLE_FLEETS);

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

                List<Model.Fleet> fleetList = new List<Model.Fleet>();
                while (reader.Read())
                {
                    fleetList.Add(constructFleetObj(reader));
                }

                return fleetList;
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

        public Model.Fleet Get(string id)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> queryParam = new Dictionary<string, string>();
                queryParam.Add("id", id);
                queryParam.Add("deleted", "0");

                mySqlCmd = GenerateQueryCmd(TABLE_FLEETS, queryParam);
                reader = PerformSqlQuery(mySqlCmd);

                if (reader.Read())
                {
                    return constructFleetObj(reader);
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

        public List<Model.Fleet> GetByCompanyId(string companyId, string limit, string skip)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                // add to job order status
                string query = string.Format("SELECT * FROM {0} WHERE deleted=0 AND company_id={1} ORDER BY creation_date DESC ",
                    TABLE_FLEETS, companyId);

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

                List<Model.Fleet> fleetList = new List<Model.Fleet>();
                while (reader.Read())
                {
                    fleetList.Add(constructFleetObj(reader));
                }

                return fleetList;
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

        public bool Update(string id, Model.Fleet payload)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                DateTime roadTaxExpiryDate = DateTime.Parse(payload.roadTaxExpiry);
                DateTime serviceDueDateDate = DateTime.Parse(payload.serviceDueDate);

                // add to job order status
                Dictionary<string, string> updateParam = new Dictionary<string, string>();
                updateParam.Add("registration_number", payload.registrationNumber);
                updateParam.Add("fleet_types_id", payload.fleetTypeId);
                updateParam.Add("road_tax_expired", roadTaxExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"));
                updateParam.Add("service_due_date", serviceDueDateDate.ToString("yyyy-MM-dd HH:mm:ss"));
                updateParam.Add("service_due_mileage", payload.serviceDueMileage.ToString());
                updateParam.Add("company_id", payload.companyId);
                updateParam.Add("remarks", payload.remarks);

                Dictionary<string, string> updateDesc = new Dictionary<string, string>();
                updateDesc.Add("id", id);

                mySqlCmd = GenerateEditCmd(TABLE_FLEETS, updateParam, updateDesc);
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

        private Model.Fleet constructFleetObj(MySqlDataReader reader)
        {
            return new Model.Fleet()
            {
                fleetId = reader["id"].ToString(),
                companyId = reader["company_id"].ToString(),
                fleetTypeId = reader["fleet_types_id"].ToString(),
                roadTaxExpiry = reader["road_tax_expired"].ToString(),
                serviceDueDate = reader["service_due_date"].ToString(),
                serviceDueMileage = (int)reader["service_due_mileage"],
                registrationNumber = reader["registration_number"].ToString(),
                remarks = reader["remarks"].ToString(),
                creationDate = reader["creation_date"].ToString(),
                lastModifiedDate = reader["last_modified_date"].ToString(),
                deleted = (int)reader["deleted"] == 0? false:true,
                enabled = (int)reader["enabled"] == 0? false:true
            };

        }
    }
}