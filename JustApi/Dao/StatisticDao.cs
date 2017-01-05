using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using JustApi.Utility;
using static JustApi.Constants.Configuration;

namespace JustApi.Dao
{
    public class StatisticDao : BaseDao
    {
        private readonly string TABLE_USER_TRACKING = "user_tracking";
        private readonly string TABLE_COMPANIES = "companies";
        private readonly string TABLE_USERS = "users";
        private readonly string TABLE_USER_COMPANY = "user_company";

        private readonly string TABLE_JOB_DELIVERY = "job_delivery";
        private readonly string TABLE_JOB_STATUS = "job_order_status";
        private readonly string TABLE_FLEETS = "fleets";

        private readonly string TABLE_JOBS = "jobs";

        public Model.DriverLocation GetByUserId(string userId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT {0}.*, {1}.id as driverId, {1}.display_name as driverName, {3}.id as companyId, {3}.name as companyName, " +
                    "{6}.id as fleetId, {6}.registration_number as fleetNumber, {6}.fleet_types_id as fleetTypeId, {4}.job_id as ongoingJobId " +
                    "FROM (SELECT *, MAX(creation_date) FROM {0} GROUP BY user_id DESC) AS {0} " +
                    "INNER JOIN {1} ON {1}.id={0}.user_id " +
                    "INNER JOIN {2} ON {2}.user_id={0}.user_id " +
                    "INNER JOIN {3} ON {3}.id={2}.company_id " +
                    "LEFT JOIN {4} ON {4}.driver_user_id={0}.user_id AND {4}.company_id={3}.id " +
                    "LEFT JOIN (SELECT * FROM {5} GROUP BY {5}.job_id) {5} ON {5}.job_id={4}.job_id AND {5}.job_status_id in (@job_status_id) " +
                    "LEFT JOIN {6} ON {6}.id={4}.fleet_id " +
                    "WHERE {0}.user_id=@driverId",
                    TABLE_USER_TRACKING, TABLE_USERS, TABLE_USER_COMPANY, TABLE_COMPANIES, TABLE_JOB_DELIVERY, TABLE_JOB_STATUS, TABLE_FLEETS);

                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@job_status_id", string.Format("{0},{1}", (int)JobStatus.Ongoing_Delivery, (int)JobStatus.Ongoing_Pickup));
                mySqlCmd.Parameters.AddWithValue("@driverId", userId);

                reader = PerformSqlQuery(mySqlCmd);
                if (reader.Read())
                {
                    return constructObj(reader);
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

        public List<Model.DriverLocation> GetByJobStatus(string jobStatus)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT {0}.*, {1}.id as driverId, {1}.display_name as driverName, {3}.id as companyId, {3}.name as companyName, " +
                    "{6}.id as fleetId, {6}.registration_number as fleetNumber, {6}.fleet_types_id as fleetTypeId, {4}.job_id as ongoingJobId " +
                    "FROM (SELECT *, MAX(creation_date) FROM {0} GROUP BY user_id DESC) AS {0} " +
                    "INNER JOIN {1} ON {1}.id={0}.user_id " +
                    "INNER JOIN {2} ON {2}.user_id={0}.user_id " +
                    "INNER JOIN {3} ON {3}.id={2}.company_id " +
                    "LEFT JOIN {4} ON {4}.driver_user_id={0}.user_id AND {4}.company_id={3}.id " +
                    "LEFT JOIN (SELECT * FROM {5} GROUP BY {5}.job_id) {5} ON {5}.job_id={4}.job_id AND {5}.job_status_id in (@job_status_id) " +
                    "LEFT JOIN {6} ON {6}.id={4}.fleet_id",
                    TABLE_USER_TRACKING, TABLE_USERS, TABLE_USER_COMPANY, TABLE_COMPANIES, TABLE_JOB_DELIVERY, TABLE_JOB_STATUS, TABLE_FLEETS);

                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@job_status_id", jobStatus);

                reader = PerformSqlQuery(mySqlCmd);
                return constructList(reader);
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

        public List<Model.DriverLocation> GetByCompany(string companyId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT {0}.*, {1}.id as driverId, {1}.display_name as driverName, {3}.id as companyId, {3}.name as companyName, " +
                    "{6}.id as fleetId, {6}.registration_number as fleetNumber, {6}.fleet_types_id as fleetTypeId, {4}.job_id as ongoingJobId " +
                    "FROM (SELECT *, MAX(creation_date) FROM {0} GROUP BY user_id DESC) AS {0} " +
                    "INNER JOIN {1} ON {1}.id={0}.user_id " +
                    "INNER JOIN {2} ON {2}.user_id={0}.user_id " +
                    "INNER JOIN {3} ON {3}.id={2}.company_id " +
                    "LEFT JOIN {4} ON {4}.driver_user_id={0}.user_id AND {4}.company_id={3}.id " +
                    "LEFT JOIN (SELECT * FROM {5} GROUP BY {5}.job_id) {5} ON {5}.job_id={4}.job_id AND {5}.job_status_id in (@job_status_id) " +
                    "LEFT JOIN {6} ON {6}.id={4}.fleet_id " +
                    "WHERE {3}.id=@companyId",
                    TABLE_USER_TRACKING, TABLE_USERS, TABLE_USER_COMPANY, TABLE_COMPANIES, TABLE_JOB_DELIVERY, TABLE_JOB_STATUS, TABLE_FLEETS);

                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@job_status_id", string.Format("{0},{1}", (int)JobStatus.Ongoing_Delivery, (int)JobStatus.Ongoing_Pickup));
                mySqlCmd.Parameters.AddWithValue("@companyId", companyId);

                reader = PerformSqlQuery(mySqlCmd);
                return constructList(reader);
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

        public List<Model.DriverLocation> GetAll()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT {0}.*, {1}.id as driverId, {1}.display_name as driverName, {3}.id as companyId, {3}.name as companyName, " +
                    "{6}.id as fleetId, {6}.registration_number as fleetNumber, {6}.fleet_types_id as fleetTypeId, {4}.job_id as ongoingJobId " + 
                    "FROM (SELECT *, MAX(creation_date) FROM {0} GROUP BY user_id DESC) AS {0} " +
                    "INNER JOIN {1} ON {1}.id={0}.user_id " +
                    "INNER JOIN {2} ON {2}.user_id={0}.user_id " + 
                    "INNER JOIN {3} ON {3}.id={2}.company_id " +
                    "LEFT JOIN {4} ON {4}.driver_user_id={0}.user_id AND {4}.company_id={3}.id " +
                    "LEFT JOIN (SELECT * FROM {5} GROUP BY {5}.job_id) {5} ON {5}.job_id={4}.job_id AND {5}.job_status_id in (@job_status_id) " +
                    "LEFT JOIN {6} ON {6}.id={4}.fleet_id",
                    TABLE_USER_TRACKING, TABLE_USERS, TABLE_USER_COMPANY, TABLE_COMPANIES, TABLE_JOB_DELIVERY, TABLE_JOB_STATUS, TABLE_FLEETS);

                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@job_status_id", string.Format("{0},{1}", (int)JobStatus.Ongoing_Delivery, (int)JobStatus.Ongoing_Pickup));

                reader = PerformSqlQuery(mySqlCmd);
                return constructList(reader);
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

        private Model.DriverLocation constructObj(MySqlDataReader reader)
        {
            Model.DriverLocation result = new Model.DriverLocation();

            result.longitude = reader.GetFloat("longitude");
            result.latitude = reader.GetFloat("latitude");
            result.lastSeen = reader["creation_date"].ToString();

            result.driver = new Model.Driver()
            {
                id = reader["driverId"].ToString(),
                name = reader["driverName"].ToString()
            };

            result.companies = new List<Model.DriverCompany>() { new Model.DriverCompany()
            {
                id = reader["companyId"].ToString(),
                name = reader["companyName"].ToString()
            }};

            result.jobs = new List<Model.OnGoingJob>()
            {
                new Model.OnGoingJob()
                {
                    fleet = new Model.Fleet()
                    {
                        fleetId = reader["fleetId"].ToString(),
                        fleetTypeId = reader["fleetTypeId"].ToString(),
                        registrationNumber = reader["fleetNumber"].ToString()
                    },
                    jobId = reader["ongoingJobId"].ToString()
                }
            };

            return result;
        }

        private List<Model.DriverLocation> constructList(MySqlDataReader reader)
        {
            List<Model.DriverLocation> driverLocationList = new List<Model.DriverLocation>();
            while (reader.Read())
            {
                var driverId = reader["driverId"].ToString();
                var previousResult = driverLocationList.Find(t => t.driver.id.CompareTo(driverId) == 0);
                if (previousResult != null)
                {
                    // previous same driver id found
                    var jobId = reader["ongoingJobId"].ToString();
                    var previousJob = previousResult.jobs.Find(t => t.jobId.CompareTo(jobId) == 0);
                    if (previousJob != null)
                    {
                        // additional job
                        previousResult.jobs.Add(new Model.OnGoingJob()
                        {
                            fleet = new Model.Fleet()
                            {
                                fleetId = reader["fleetId"].ToString(),
                                fleetTypeId = reader["fleetTypeId"].ToString(),
                                registrationNumber = reader["fleetNumber"].ToString()
                            },
                            jobId = reader["ongoingJobId"].ToString()
                        });
                    }
                    else
                    {
                        // only add the company info
                        previousResult.companies.Add(new Model.DriverCompany()
                        {
                            id = reader["companyId"].ToString(),
                            name = reader["companyName"].ToString()
                        });
                    }

                }
                else
                {
                    driverLocationList.Add(constructObj(reader));
                }
            }

            return driverLocationList;
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public Model.AdminStatistics getAdminStatistic(string dateStart, string dateEnd)
        {

            // prepare the listing
            Model.AdminStatistics result = new Model.AdminStatistics();
            result.total = new Model.Statistic();
            result.statistics = new Dictionary<string, Model.Statistic>();

            // get from database
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;

            var datetimeStart = DateTime.Parse(dateStart);
            var datetimeEnd = DateTime.Parse(dateEnd);
            foreach (DateTime day in EachDay(datetimeStart, datetimeEnd))
            {
                var statistic = new Model.Statistic();
                result.statistics.Add(day.ToShortDateString(), statistic);

                string query = string.Format("SELECT (SELECT COUNT(*) FROM {0} WHERE DATE(creation_date) = '{1}') AS userCount, " +
                    "(SELECT COUNT(*) FROM {2} WHERE DATE(creation_date) = '{1}') AS companyCount, " +
                    "(SELECT COUNT(*) FROM {3} WHERE DATE(creation_date) = '{1}') AS jobCount, " +
                    "(SELECT COALESCE(SUM(amount), 0) FROM {3} WHERE DATE(creation_date) = '{1}') AS revenue, " +
                    "(SELECT COALESCE(SUM(amount), 0)*0.1 FROM {3} WHERE DATE(creation_date) = '{1}') AS profit, " +
                    "(SELECT COUNT(*) FROM {4} WHERE DATE(creation_date) = '{1}') AS fleetCount ",
                    TABLE_USERS, day.ToString("yyyy-MM-dd"), TABLE_COMPANIES, TABLE_JOBS, TABLE_FLEETS);

                mySqlCmd = new MySqlCommand(query);
                reader = PerformSqlQuery(mySqlCmd);
                if (reader.Read())
                {
                    statistic.numUniqueUsers = reader.GetInt32("userCount");
                    statistic.numPartners = reader.GetInt32("companyCount");
                    statistic.numJobs = reader.GetInt32("jobCount");
                    statistic.numProfits = reader.GetFloat("profit");
                    statistic.numRevenues = reader.GetFloat("revenue");
                    statistic.numFleets = reader.GetInt32("fleetCount");
                }

                CleanUp(reader, mySqlCmd);
            }


            // get all the figures
            string totalQuery = string.Format("SELECT (SELECT COUNT(*) FROM {0} WHERE DATE(creation_date) between '{4}' AND '{5}') AS userCount, " +
                    "(SELECT COUNT(*) FROM {1} WHERE DATE(creation_date) between '{4}' AND '{5}') AS companyCount, " +
                    "(SELECT COUNT(*) FROM {2} WHERE DATE(creation_date) between '{4}' AND '{5}') AS jobCount, " +
                    "(SELECT COALESCE(SUM(amount), 0) FROM {2} WHERE DATE(creation_date) between '{4}' AND '{5}') AS revenue, " +
                    "(SELECT COALESCE(SUM(amount), 0)*0.1 FROM {2} WHERE DATE(creation_date) between '{4}' AND '{5}') AS profit, " +
                    "(SELECT COUNT(*) FROM {3} WHERE DATE(creation_date) between '{4}' AND '{5}') AS fleetCount ",
                    TABLE_USERS, TABLE_COMPANIES, TABLE_JOBS, TABLE_FLEETS, datetimeStart.ToString("yyyy-MM-dd"), datetimeEnd.AddDays(1).ToString("yyyy-MM-dd"));

            mySqlCmd = new MySqlCommand(totalQuery);
            reader = PerformSqlQuery(mySqlCmd);
            if (reader.Read())
            {
                result.total.numUniqueUsers = reader.GetInt32("userCount");
                result.total.numPartners = reader.GetInt32("companyCount");
                result.total.numJobs = reader.GetInt32("jobCount");
                result.total.numProfits = reader.GetFloat("profit");
                result.total.numRevenues = reader.GetFloat("revenue");
                result.total.numFleets = reader.GetInt32("fleetCount");
            }

            CleanUp(reader, mySqlCmd);
            return result;
        }
    }
}