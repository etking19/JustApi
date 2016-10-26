using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class AddressDao : BaseDao
    {
        public enum EType
        {
            From = 0,
            To = 1
        }

        private readonly string TABLE_ADD = "addresses";
        private readonly string TABLE_JOB_FROM = "job_from";
        private readonly string TABLE_JOB_TO = "job_to";
        private readonly string TABLE_JOB = "jobs";

        public List<Model.Address> Get(string userId, string limit, string skip, EType type)
        {
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            try
            {
                var destinationTable = (type == EType.From ? TABLE_JOB_FROM : TABLE_JOB_TO);
                string query = string.Format("SELECT {2}.*, {0}.customer_name as name, {0}.customer_contact as contact FROM {0} " +
                    "INNER JOIN {1} ON {0}.job_id={1}.id AND {1}.owner_id={3} " +
                    "INNER JOIN {2} ON {2}.id={0}.address_id " +
                    "ORDER BY id DESC ",
                    destinationTable, TABLE_JOB, TABLE_ADD, userId);

                if (limit != null)
                {
                    query += string.Format("LIMIT {0} ", limit);
                }

                if (skip != null)
                {
                    query += string.Format("OFFSET {0} ", skip);
                }

                command = new MySqlCommand(query);
                reader = PerformSqlQuery(command);

                List<Model.Address> addressList = new List<Model.Address>();
                while (reader.Read())
                {
                    addressList.Add(new Model.Address()
                    {
                        addressId = reader["id"].ToString(),
                        address1 = reader["add_1"].ToString(),
                        address2 = reader["add_2"].ToString(),
                        address3 = reader["add_3"].ToString(),
                        contactPerson = reader["name"].ToString(),
                        contact = reader["contact"].ToString(),
                        countryId = reader["country_id"].ToString(),
                        stateId = reader["state_id"].ToString(),
                        gpsLatitude = (float)reader["gps_latitude"],
                        gpsLongitude = (float)reader["gps_longitude"],
                        postcode = reader["postcode"].ToString(),
                        creationDate = reader["creation_date"].ToString(),
                    });
                }

                return addressList;
            }
            catch (Exception e)
            {
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Error, e.Message);
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, e.StackTrace);
            }
            finally
            {
                CleanUp(reader, command);
            }

            return null;
        }

        public string Add(Model.Address payload, string jobId, string customerName, string contact, EType type)
        {
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            try
            {
                var destinationTable = (type == EType.From ? TABLE_JOB_FROM : TABLE_JOB_TO);
                string query = string.Format("INSERT into {0} (add_1, add_2, add_3, state_id, country_id, postcode, gps_longitude, gps_latitude, create_by)" +
                    "VALUES (@add_1, @add_2, @add_3, @state_id, @country_id, @postcode, @gps_longitude, @gps_latitude, @create_by); " +
                    "INSERT into {1} (address_id, job_id, customer_name, customer_contact) " +
                    "VALUES (LAST_INSERT_ID(), @job_id, @customer_name, @customer_contact)",
                    TABLE_ADD, destinationTable);

                command = new MySqlCommand(query);
                command.Parameters.AddWithValue("@add_1", payload.address1);
                command.Parameters.AddWithValue("@add_2", payload.address2);
                command.Parameters.AddWithValue("@add_3", payload.address3);
                command.Parameters.AddWithValue("@state_id", payload.stateId);
                command.Parameters.AddWithValue("@country_id", payload.countryId);
                command.Parameters.AddWithValue("@postcode", payload.postcode);
                command.Parameters.AddWithValue("@gps_longitude", payload.gpsLongitude);
                command.Parameters.AddWithValue("@gps_latitude", payload.gpsLatitude);
                command.Parameters.AddWithValue("@create_by", payload.createdBy);

                command.Parameters.AddWithValue("@job_id", jobId);
                command.Parameters.AddWithValue("@customer_name", customerName);
                command.Parameters.AddWithValue("@customer_contact", contact);

                return PerformSqlNonQuery(command).ToString();
            }
            catch (Exception e)
            {
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Error, e.Message);
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, e.StackTrace);
            }
            finally
            {
                CleanUp(reader, command);
            }

            return null;
        }
    }
}