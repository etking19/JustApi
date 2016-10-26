using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class JobDetailsDao : BaseDao
    {
        private readonly string TABLE_NAME_JOB = "jobs";
        private readonly string TABLE_NAME_JOBDELIVERY = "job_delivery";
        private readonly string TABLE_NAME_ADDFROM = "job_from";
        private readonly string TABLE_NAME_ADDTO = "job_to";
        private readonly string TABLE_NAME_JOBORDERSTATUS = "job_order_status";
        private readonly string TABLE_NAME_ADDRESS = "addresses";
        private readonly string TABLE_NAME_DECLINED = "job_delivery_declined";

        public string AddOrder(string jobId, string modifyBy)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                // add to job order status
                Dictionary<string, string>  insertParam = new Dictionary<string, string>();
                insertParam.Add("job_id", jobId);
                insertParam.Add("modify_by", modifyBy);
                mySqlCmd = GenerateAddCmd(TABLE_NAME_JOBORDERSTATUS, insertParam);
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

        public string Add(Model.JobDetails payload)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                DateTime date = DateTime.Parse(payload.deliveryDate);

                Dictionary<string, string> insertParam = new Dictionary<string, string>();
                insertParam.Add("owner_id", payload.ownerUserId);
                insertParam.Add("job_type_id", payload.jobTypeId);
                insertParam.Add("amount", payload.amount.ToString());
                insertParam.Add("cash_on_delivery", payload.cashOnDelivery ? "1" : "0");
                insertParam.Add("worker_assistance", payload.workerAssistant.ToString());
                insertParam.Add("fleet_type_id", payload.fleetTypeId);
                insertParam.Add("delivery_date", date.ToString("yyyy-MM-dd HH:mm:ss"));
                insertParam.Add("remarks", payload.remarks.ToString());
                insertParam.Add("created_by", payload.createdBy);
                insertParam.Add("modify_by", payload.createdBy);

                mySqlCmd = GenerateAddCmd(TABLE_NAME_JOB, insertParam);
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

        /// <summary>
        /// return total amount paid
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="amountPaid"></param>
        /// <returns></returns>
        public float UpdatePaidAmount(string jobId, float amountPaid)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("UPDATE {0} SET amount_paid= amount_paid + @amountPaid WHERE id=@id; SELECT amount_paid FROM {0} WHERE id=@id", 
                    TABLE_NAME_JOB);

                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@amountPaid", amountPaid);
                mySqlCmd.Parameters.AddWithValue("@id", jobId);

                
                reader = PerformSqlQuery(mySqlCmd);
                if(reader.Read())
                {
                    return reader.GetFloat("amount_paid");
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

        public bool Update(Model.JobDetails payload)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                DateTime date = DateTime.Parse(payload.deliveryDate);

                Dictionary<string, string> updateParam = new Dictionary<string, string>();
                updateParam.Add("owner_id", payload.ownerUserId);
                updateParam.Add("job_type_id", payload.jobTypeId);
                updateParam.Add("amount", payload.amount.ToString());
                updateParam.Add("cash_on_delivery", payload.cashOnDelivery ? "1" : "0");
                updateParam.Add("fleet_type_id", payload.fleetTypeId);
                updateParam.Add("worker_assistance", payload.workerAssistant.ToString());
                updateParam.Add("delivery_date", date.ToString("yyyy-MM-dd HH:mm:ss"));
                updateParam.Add("remarks", payload.remarks);
                updateParam.Add("modify_by", payload.modifiedBy);

                Dictionary<string, string> destinationParam = new Dictionary<string, string>();
                destinationParam.Add("id", payload.jobId);

                mySqlCmd = GenerateEditCmd(TABLE_NAME_JOB, updateParam, destinationParam);
                return (PerformSqlNonQuery(mySqlCmd) != 0);
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

        public bool Delete(string id)
        {
            MySqlCommand mySqlCmd = null;
            try
            {
                Dictionary<string, string> removeParams = new Dictionary<string, string>();
                removeParams.Add("id", id);

                mySqlCmd = GenerateSoftDelete(TABLE_NAME_JOB, removeParams);
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

        public Model.JobDetails GetByJobId(string jobId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT * FROM {0} " +
                    "LEFT JOIN (SELECT {3}.add_1 as add_from_1, {3}.add_2 as add_from_2, {3}.add_3 as add_from_3, {3}.state_id as state_from, {3}.country_id as country_from, {3}.postcode as postcode_from, {3}.gps_longitude as longitude_from, {3}.gps_latitude as latitude_from, {1}.*, {1}.address_id as addFromId, {1}.customer_name as customerFrom, {1}.customer_contact as contactFrom FROM {1} INNER JOIN {3} ON {1}.address_id={3}.id) addFrom ON {0}.id=addFrom.job_id " +
                    "LEFT JOIN (SELECT {3}.add_1 as add_to_1, {3}.add_2 as add_to_2, {3}.add_3 as add_to_3, {3}.state_id as state_to, {3}.country_id as country_to, {3}.postcode as postcode_to, {3}.gps_longitude as longitude_to, {3}.gps_latitude as latitude_to, {2}.*, {2}.address_id as addToId, {2}.customer_name as customerTo, {2}.customer_contact as contactTo FROM {2} INNER JOIN {3} ON {2}.address_id={3}.id) addTo ON {0}.id=addTo.job_id " +
                    "INNER JOIN (SELECT job_status_id, job_id as jsId FROM {4} a inner join (select max(last_modified_date) last_modified_date, job_id as jsId2 from {4} group by job_id) b ON a.job_id=b.jsId2 AND a.last_modified_date=b.last_modified_date) jobStatus ON jobStatus.jsId={0}.id " +
                    "WHERE {0}.deleted=0 AND {0}.id=@jobId;", 
                    TABLE_NAME_JOB, TABLE_NAME_ADDFROM, TABLE_NAME_ADDTO, TABLE_NAME_ADDRESS, TABLE_NAME_JOBORDERSTATUS);

                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@jobId", jobId);

                reader = PerformSqlQuery(mySqlCmd);
                Model.JobDetails result = null;
                List<Model.Address> fromAddList = new List<Model.Address>();
                List<Model.Address> toAddList = new List<Model.Address>();
                while (reader.Read())
                {
                    if(result == null)
                    {
                        result = new Model.JobDetails()
                        {
                            jobId = reader["id"].ToString(),
                            ownerUserId = reader["owner_id"].ToString(),
                            jobTypeId = reader["job_type_id"].ToString(),
                            fleetTypeId = reader["fleet_type_id"].ToString(),
                            amount = (float)reader["amount"],
                            amountPaid = (float)reader["amount_paid"],
                            cashOnDelivery = (int)reader["cash_on_delivery"] == 0 ? false : true,
                            workerAssistant = (int)reader["worker_assistance"],
                            deliveryDate = reader["delivery_date"].ToString(),
                            remarks = (string)reader["remarks"],
                            enabled = (int)reader["enabled"] == 0 ? false : true,
                            deleted = (int)reader["deleted"] == 0 ? false : true,
                            createdBy = reader["created_by"].ToString(),
                            creationDate = reader["creation_date"].ToString(),
                            modifiedBy = reader["modify_by"].ToString(),
                            lastModifiedDate = reader["last_modified_date"].ToString(),
                            jobStatusId = reader["job_status_id"] == null ? null : reader["job_status_id"].ToString(),
                            addressFrom = new List<Model.Address>(),
                            addressTo = new List<Model.Address>()
                        };
                    }

                    try
                    {
                        result.addressFrom.Add(new Model.Address()
                        {
                            addressId = reader["addFromId"].ToString(),
                            address1 = (string)reader["add_from_1"],
                            address2 = (string)reader["add_from_2"],
                            address3 = (string)reader["add_from_3"],
                            stateId = reader["state_from"].ToString(),
                            countryId = reader["country_from"].ToString(),
                            postcode = (string)reader["postcode_from"],
                            gpsLongitude = (float)reader["longitude_from"],
                            gpsLatitude = (float)reader["latitude_from"],
                            contactPerson = (string)reader["customerFrom"],
                            contact = (string)reader["contactFrom"]
                        });
                    }
                    catch (Exception)
                    {
                        // possible do not have from
                    }

                    try
                    {
                        result.addressTo.Add(new Model.Address()
                        {
                            addressId = reader["addToId"].ToString(),
                            address1 = (string)reader["add_to_1"],
                            address2 = (string)reader["add_to_2"],
                            address3 = (string)reader["add_to_3"],
                            stateId = reader["state_to"].ToString(),
                            countryId = reader["country_to"].ToString(),
                            postcode = (string)reader["postcode_to"],
                            gpsLongitude = (float)reader["longitude_to"],
                            gpsLatitude = (float)reader["latitude_to"],
                            contactPerson = (string)reader["customerTo"],
                            contact = (string)reader["contactTo"]
                        });
                    }
                    catch (Exception)
                    {
                        // possible do not have to
                    }

                }

                return result;
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

        public List<Model.JobDetails> GetByJobTypeId(string jobTypeId, string dateFrom, string dateTo, string limit, string skip)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT * FROM (SELECT * FROM {0} WHERE deleted=0 AND job_type_id=@jobTypeId AND (delivery_date >= @dateFrom OR @dateFrom IS NULL) AND (delivery_date <= @dateTo OR @dateTo IS NULL) ORDER BY delivery_date DESC ", 
                    TABLE_NAME_JOB);

                if (limit != null)
                {
                    query += string.Format("LIMIT {0} ", limit);
                }

                if (skip != null)
                {
                    query += string.Format("OFFSET {0} ", skip);
                }

                query += ") jobDetails ";

                // job_from
                query += string.Format("LEFT JOIN (SELECT {1}.add_1 as add_from_1, {1}.add_2 as add_from_2, {1}.add_3 as add_from_3, {1}.state_id as state_from, {1}.country_id as country_from, {1}.postcode as postcode_from, {1}.gps_longitude as longitude_from, {1}.gps_latitude as latitude_from, {0}.*, {0}.address_id as addFromId, {0}.customer_name as customerFrom, {0}.customer_contact as contactFrom FROM {0} INNER JOIN {1} ON {0}.address_id={1}.id) addFrom ON jobDetails.id=addFrom.job_id ",
                    TABLE_NAME_ADDFROM, TABLE_NAME_ADDRESS);

                // job to
                query += string.Format("LEFT JOIN (SELECT {1}.add_1 as add_to_1, {1}.add_2 as add_to_2, {1}.add_3 as add_to_3, {1}.state_id as state_to, {1}.country_id as country_to, {1}.postcode as postcode_to, {1}.gps_longitude as longitude_to, {1}.gps_latitude as latitude_to, {0}.*, {0}.address_id as addToId, {0}.customer_name as customerTo, {0}.customer_contact as contactTo FROM {0} INNER JOIN {1} ON {0}.address_id={1}.id) addTo ON jobDetails.id=addTo.job_id ",
                    TABLE_NAME_ADDTO, TABLE_NAME_ADDRESS);

                // job order status
                query += string.Format("INNER JOIN (SELECT job_status_id, job_id as jsId FROM {0} a " +
                                    "inner join (select max(last_modified_date) last_modified_date, job_id as jsId2 from {0} group by job_id) b ON a.job_id=b.jsId2 AND a.last_modified_date=b.last_modified_date) jobStatus ON  jobStatus.jsId=jobDetails.id ", TABLE_NAME_JOBORDERSTATUS);

                // reverse order
                query += "ORDER BY creation_date DESC;";


                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@jobTypeId", jobTypeId);
                mySqlCmd.Parameters.AddWithValue("@dateFrom", dateFrom);
                mySqlCmd.Parameters.AddWithValue("@dateTo", dateTo);

                reader = PerformSqlQuery(mySqlCmd);
                return getDetailsList(reader);
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

        public List<Model.JobDetails> GetByDateRange(string dateFrom, string dateTo, string limit, string skip)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT * FROM (SELECT * FROM {0} WHERE deleted=0 AND (delivery_date >= @dateFrom OR @dateFrom IS NULL) AND (delivery_date <= @dateTo OR @dateTo IS NULL) ORDER BY delivery_date DESC ",
                    TABLE_NAME_JOB);

                if (limit != null)
                {
                    query += string.Format("LIMIT {0} ", limit);
                }

                if (skip != null)
                {
                    query += string.Format("OFFSET {0} ", skip);
                }

                query += ") jobDetails ";

                // job_from
                query += string.Format("LEFT JOIN (SELECT {1}.add_1 as add_from_1, {1}.add_2 as add_from_2, {1}.add_3 as add_from_3, {1}.state_id as state_from, {1}.country_id as country_from, {1}.postcode as postcode_from, {1}.gps_longitude as longitude_from, {1}.gps_latitude as latitude_from, {0}.*, {0}.address_id as addFromId, {0}.customer_name as customerFrom, {0}.customer_contact as contactFrom FROM {0} INNER JOIN {1} ON {0}.address_id={1}.id) addFrom ON jobDetails.id=addFrom.job_id ",
                    TABLE_NAME_ADDFROM, TABLE_NAME_ADDRESS);

                // job to
                query += string.Format("LEFT JOIN (SELECT {1}.add_1 as add_to_1, {1}.add_2 as add_to_2, {1}.add_3 as add_to_3, {1}.state_id as state_to, {1}.country_id as country_to, {1}.postcode as postcode_to, {1}.gps_longitude as longitude_to, {1}.gps_latitude as latitude_to, {0}.*, {0}.address_id as addToId, {0}.customer_name as customerTo, {0}.customer_contact as contactTo FROM {0} INNER JOIN {1} ON {0}.address_id={1}.id) addTo ON jobDetails.id=addTo.job_id ",
                    TABLE_NAME_ADDTO, TABLE_NAME_ADDRESS);

                // job order status
                query += string.Format("INNER JOIN (SELECT job_status_id, job_id as jsId FROM {0} a " +
                                    "inner join (select max(last_modified_date) last_modified_date, job_id as jsId2 from {0} group by job_id) b ON a.job_id=b.jsId2 AND a.last_modified_date=b.last_modified_date) jobStatus ON  jobStatus.jsId=jobDetails.id ", TABLE_NAME_JOBORDERSTATUS);

                // reverse order
                query += "ORDER BY creation_date DESC;";


                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@dateFrom", dateFrom);
                mySqlCmd.Parameters.AddWithValue("@dateTo", dateTo);

                reader = PerformSqlQuery(mySqlCmd);
                return getDetailsList(reader);
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

        public List<Model.JobDetails> GetByOwnerId(string ownerId, string dateFrom, string dateTo, string limit, string skip)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT * FROM (SELECT * FROM {0} WHERE deleted=0 AND owner_id=@ownerId AND (delivery_date >= @dateFrom OR @dateFrom IS NULL) AND (delivery_date <= @dateTo OR @dateTo IS NULL) ORDER BY delivery_date DESC ", 
                    TABLE_NAME_JOB);

                if (limit != null)
                {
                    query += string.Format("LIMIT {0} ", limit);
                }

                if (skip != null)
                {
                    query += string.Format("OFFSET {0} ", skip);
                }

                query += ") jobDetails ";

                // job_from
                query += string.Format("LEFT JOIN (SELECT {1}.add_1 as add_from_1, {1}.add_2 as add_from_2, {1}.add_3 as add_from_3, {1}.state_id as state_from, {1}.country_id as country_from, {1}.postcode as postcode_from, {1}.gps_longitude as longitude_from, {1}.gps_latitude as latitude_from, {0}.*, {0}.address_id as addFromId, {0}.customer_name as customerFrom, {0}.customer_contact as contactFrom FROM {0} INNER JOIN {1} ON {0}.address_id={1}.id) addFrom ON jobDetails.id=addFrom.job_id ",
                    TABLE_NAME_ADDFROM, TABLE_NAME_ADDRESS);

                // job to
                query += string.Format("LEFT JOIN (SELECT {1}.add_1 as add_to_1, {1}.add_2 as add_to_2, {1}.add_3 as add_to_3, {1}.state_id as state_to, {1}.country_id as country_to, {1}.postcode as postcode_to, {1}.gps_longitude as longitude_to, {1}.gps_latitude as latitude_to, {0}.*, {0}.address_id as addToId, {0}.customer_name as customerTo, {0}.customer_contact as contactTo FROM {0} INNER JOIN {1} ON {0}.address_id={1}.id) addTo ON jobDetails.id=addTo.job_id ",
                    TABLE_NAME_ADDTO, TABLE_NAME_ADDRESS);

                // job order status
                query += string.Format("INNER JOIN (SELECT job_status_id, job_id as jsId FROM {0} a " +
                                    "inner join (select max(last_modified_date) last_modified_date, job_id as jsId2 from {0} group by job_id) b ON a.job_id=b.jsId2 AND a.last_modified_date=b.last_modified_date AND job_status_id <> 1) jobStatus ON  jobStatus.jsId=jobDetails.id ", TABLE_NAME_JOBORDERSTATUS);

                // reverse order
                query += "ORDER BY creation_date DESC;";


                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@ownerId", ownerId);
                mySqlCmd.Parameters.AddWithValue("@dateFrom", dateFrom);
                mySqlCmd.Parameters.AddWithValue("@dateTo", dateTo);

                reader = PerformSqlQuery(mySqlCmd);
                return getDetailsList(reader);
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

        public List<Model.JobDetails> Get(string limit, string skip)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT * FROM (SELECT * FROM {0} WHERE deleted=0 ORDER BY delivery_date DESC ", TABLE_NAME_JOB);

                if (limit != null)
                {
                    query += string.Format("LIMIT {0} ", limit);
                }

                if (skip != null)
                {
                    query += string.Format("OFFSET {0} ", skip);
                }

                query += ") jobDetails ";

                // job_from
                query += string.Format("LEFT JOIN (SELECT {1}.add_1 as add_from_1, {1}.add_2 as add_from_2, {1}.add_3 as add_from_3, {1}.state_id as state_from, {1}.country_id as country_from, {1}.postcode as postcode_from, {1}.gps_longitude as longitude_from, {1}.gps_latitude as latitude_from, {0}.*, {0}.address_id as addFromId, {0}.customer_name as customerFrom, {0}.customer_contact as contactFrom FROM {0} INNER JOIN {1} ON {0}.address_id={1}.id) addFrom ON jobDetails.id=addFrom.job_id ",
                    TABLE_NAME_ADDFROM, TABLE_NAME_ADDRESS);

                // job to
                query += string.Format("LEFT JOIN (SELECT {1}.add_1 as add_to_1, {1}.add_2 as add_to_2, {1}.add_3 as add_to_3, {1}.state_id as state_to, {1}.country_id as country_to, {1}.postcode as postcode_to, {1}.gps_longitude as longitude_to, {1}.gps_latitude as latitude_to, {0}.*, {0}.address_id as addToId, {0}.customer_name as customerTo, {0}.customer_contact as contactTo FROM {0} INNER JOIN {1} ON {0}.address_id={1}.id) addTo ON jobDetails.id=addTo.job_id ",
                    TABLE_NAME_ADDTO, TABLE_NAME_ADDRESS);

                // job order status
                query += string.Format("INNER JOIN (SELECT job_status_id, job_id as jsId FROM {0} a " +
                                    "inner join (select max(last_modified_date) last_modified_date, job_id as jsId2 from {0} group by job_id) b ON a.job_id=b.jsId2 AND a.last_modified_date=b.last_modified_date) jobStatus ON  jobStatus.jsId=jobDetails.id ", TABLE_NAME_JOBORDERSTATUS);

                // reverse order
                query += "ORDER BY creation_date DESC;";


                mySqlCmd = new MySqlCommand(query);
                reader = PerformSqlQuery(mySqlCmd);
                return getDetailsList(reader);
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

        private List<Model.JobDetails> getDetailsList(MySqlDataReader reader)
        {
            List<Model.JobDetails> jobDetailsList = new List<Model.JobDetails>();
            while (reader.Read())
            {
                var jobId = reader["id"].ToString();
                var existedResult = jobDetailsList.Find(x => x.jobId == jobId);
                if (existedResult != null)
                {
                    // already have same job id
                    var addFromObj = new Model.Address();
                    try
                    {
                        addFromObj.addressId = reader["addFromId"].ToString();
                        addFromObj.address1 = (string)reader["add_from_1"];
                        addFromObj.address2 = (string)reader["add_from_2"];
                        addFromObj.address3 = (string)reader["add_from_3"];
                        addFromObj.stateId = reader["state_from"].ToString();
                        addFromObj.countryId = reader["country_from"].ToString();
                        addFromObj.postcode = (string)reader["postcode_from"];
                        addFromObj.gpsLongitude = (float)reader["longitude_from"];
                        addFromObj.gpsLatitude = (float)reader["latitude_from"];
                        addFromObj.contactPerson = (string)reader["customerFrom"];
                        addFromObj.contact = (string)reader["contactFrom"];

                        existedResult.addressFrom.Add(addFromObj);
                    }
                    catch (Exception)
                    {
                        // possible do not have from address
                    }

                    var addToObj = new Model.Address();
                    try
                    {
                        addToObj.addressId = reader["addToId"].ToString();
                        addToObj.address1 = (string)reader["add_to_1"];
                        addToObj.address2 = (string)reader["add_to_2"];
                        addToObj.address3 = (string)reader["add_to_3"];
                        addToObj.stateId = reader["state_to"].ToString();
                        addToObj.countryId = reader["country_to"].ToString();
                        addToObj.postcode = (string)reader["postcode_to"];
                        addToObj.gpsLongitude = (float)reader["longitude_to"];
                        addToObj.gpsLatitude = (float)reader["latitude_to"];
                        addToObj.contactPerson = (string)reader["customerTo"];
                        addToObj.contact = (string)reader["contactTo"];

                        existedResult.addressTo.Add(addToObj);
                    }
                    catch (Exception)
                    {
                        // possible do not have to address
                    }

                    continue;
                }

                // new job id
                var addFromList = new List<Model.Address>();
                var addFromObjNew = new Model.Address();
                try
                {
                    addFromObjNew.addressId = reader["addFromId"].ToString();
                    addFromObjNew.address1 = (string)reader["add_from_1"];
                    addFromObjNew.address2 = (string)reader["add_from_2"];
                    addFromObjNew.address3 = (string)reader["add_from_3"];
                    addFromObjNew.stateId = reader["state_from"].ToString();
                    addFromObjNew.countryId = reader["country_from"].ToString();
                    addFromObjNew.postcode = (string)reader["postcode_from"];
                    addFromObjNew.gpsLongitude = (float)reader["longitude_from"];
                    addFromObjNew.gpsLatitude = (float)reader["latitude_from"];
                    addFromObjNew.contactPerson = (string)reader["customerFrom"];
                    addFromObjNew.contact = (string)reader["contactFrom"];

                    addFromList.Add(addFromObjNew);
                }
                catch (Exception)
                {
                    // possible do not have from address
                }

                var addToList = new List<Model.Address>();
                var addToObjNew = new Model.Address();
                try
                {
                    addToObjNew.addressId = reader["addToId"].ToString();
                    addToObjNew.address1 = (string)reader["add_to_1"];
                    addToObjNew.address2 = (string)reader["add_to_2"];
                    addToObjNew.address3 = (string)reader["add_to_3"];
                    addToObjNew.stateId = reader["state_to"].ToString();
                    addToObjNew.countryId = reader["country_to"].ToString();
                    addToObjNew.postcode = (string)reader["postcode_to"];
                    addToObjNew.gpsLongitude = (float)reader["longitude_to"];
                    addToObjNew.gpsLatitude = (float)reader["latitude_to"];
                    addToObjNew.contactPerson = (string)reader["customerTo"];
                    addToObjNew.contact = (string)reader["contactTo"];

                    addToList.Add(addToObjNew);
                }
                catch (Exception)
                {
                    // possible do not have to address
                }

                jobDetailsList.Add(new Model.JobDetails()
                {
                    jobId = jobId,
                    ownerUserId = reader["owner_id"].ToString(),
                    jobTypeId = reader["job_type_id"].ToString(),
                    fleetTypeId = reader["fleet_type_id"].ToString(),
                    amount = (float)reader["amount"],
                    amountPaid = (float)reader["amount_paid"],
                    cashOnDelivery = (int)reader["cash_on_delivery"] == 0 ? false : true,
                    workerAssistant = (int)reader["worker_assistance"],
                    deliveryDate = reader["delivery_date"].ToString(),
                    remarks = (string)reader["remarks"],
                    enabled = (int)reader["enabled"] == 0 ? false : true,
                    deleted = (int)reader["deleted"] == 0 ? false : true,
                    createdBy = reader["created_by"].ToString(),
                    creationDate = reader["creation_date"].ToString(),
                    modifiedBy = reader["modify_by"].ToString(),
                    lastModifiedDate = reader["last_modified_date"].ToString(),
                    jobStatusId = reader["job_status_id"] == null ? null : reader["job_status_id"].ToString(),
                    addressFrom = addFromList,
                    addressTo = addToList
                });
            }

            return jobDetailsList;
        }

        public List<Model.JobDetails> GetOpenJobs(string companyId, string limit, string skip)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT * FROM (SELECT * FROM {0} WHERE id NOT IN (SELECT job_id FROM {1}) AND jobs.deleted=0 AND jobs.enabled<>0 ", 
                    TABLE_NAME_JOB, TABLE_NAME_JOBDELIVERY);

                if (companyId != null)
                {
                    query += string.Format("AND id NOT IN (SELECT job_id FROM {0} WHERE company_id={1}) ORDER BY creation_date DESC ",
                        TABLE_NAME_DECLINED, companyId);
                }

                if (limit != null)
                {
                    query += string.Format("LIMIT {0} ", limit);
                }

                if (skip != null)
                {
                    query += string.Format("OFFSET {0} ", skip);
                }

                query += ") jobDetails ";

                // job_from
                query += string.Format("LEFT JOIN (SELECT {1}.add_1 as add_from_1, {1}.add_2 as add_from_2, {1}.add_3 as add_from_3, {1}.state_id as state_from, {1}.country_id as country_from, {1}.postcode as postcode_from, {1}.gps_longitude as longitude_from, {1}.gps_latitude as latitude_from, {0}.*, {0}.address_id as addFromId, {0}.customer_name as customerFrom, {0}.customer_contact as contactFrom FROM {0} INNER JOIN {1} ON {0}.address_id={1}.id) addFrom ON jobDetails.id=addFrom.job_id ",
                    TABLE_NAME_ADDFROM, TABLE_NAME_ADDRESS);

                // job to
                query += string.Format("LEFT JOIN (SELECT {1}.add_1 as add_to_1, {1}.add_2 as add_to_2, {1}.add_3 as add_to_3, {1}.state_id as state_to, {1}.country_id as country_to, {1}.postcode as postcode_to, {1}.gps_longitude as longitude_to, {1}.gps_latitude as latitude_to, {0}.*, {0}.address_id as addToId, {0}.customer_name as customerTo, {0}.customer_contact as contactTo FROM {0} INNER JOIN {1} ON {0}.address_id={1}.id) addTo ON jobDetails.id=addTo.job_id ",
                    TABLE_NAME_ADDTO, TABLE_NAME_ADDRESS);

                // job order status
                query += string.Format("LEFT JOIN (SELECT job_status_id, job_id as jsId FROM {0} a " +
                    "inner join (select max(last_modified_date) last_modified_date, job_id as jsId2 from {0} group by job_id) b ON a.job_id=b.jsId2 AND a.last_modified_date=b.last_modified_date AND job_status_id <> 1) jobStatus ON  jobStatus.jsId=jobDetails.id ", TABLE_NAME_JOBORDERSTATUS);

                //if (companyId != null)
                //{
                //    query += string.Format("LEFT JOIN {0} ON {0}.job_id= jobDetails.id AND {0}.company_id={1} WHERE {0}.id IS NULL ",
                //        TABLE_NAME_DECLINED, companyId);
                //}

                // reverse order
                query += "ORDER BY creation_date DESC;";


                mySqlCmd = new MySqlCommand(query);
                reader = PerformSqlQuery(mySqlCmd);
                return getDetailsList(reader);
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

        public Model.ResponseJobOrderStatus GetJobStatus(string jobId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT * FROM {0} " +
                    "INNER JOIN {1} ON {0}.id={1}.job_id " +
                    "WHERE {0}.id={2}",
                    TABLE_NAME_JOB, TABLE_NAME_JOBORDERSTATUS, jobId);

                mySqlCmd = new MySqlCommand(query);
                reader = PerformSqlQuery(mySqlCmd);

                Model.ResponseJobOrderStatus result = null;
                List<Model.JobOrderStatus> jobOrderStatusList = new List<Model.JobOrderStatus>();
                while (reader.Read())
                {
                    if(result == null)
                    {
                        result = new Model.ResponseJobOrderStatus()
                        {
                            job_id = reader["id"].ToString(),
                            orderStatus = new List<Model.JobOrderStatus>(),
                            enabled = true,
                            deleted = true
                        };
                    }

                    result.enabled &= ((int)reader["enabled"] == 0 ? false : true);
                    result.deleted &= ((int)reader["deleted"] == 0 ? false : true);

                    result.orderStatus.Add(new Model.JobOrderStatus()
                    {
                        job_status_id = reader["job_status_id"].ToString(),
                        modify_by = reader["modify_by"].ToString(),
                        last_modified_date = reader["last_modified_date"].ToString()
                    });
                }

                return result;
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