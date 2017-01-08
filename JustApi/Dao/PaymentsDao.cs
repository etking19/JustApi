using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Dao;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class PaymentsDao : BaseDao
    {
        private readonly string TABLE_PAYMENTS = "payments";

        public string AddOrUpdate(string jobId, Model.BillPlz.Bill bill)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                DateTime dueDate = new DateTime();
                try
                {
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, bill.due_at);
                    dueDate = DateTime.ParseExact(bill.due_at, "yyyy-MM-dd", null);
                }
                catch (Exception e)
                {
                    // do nothing
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, e.Message);
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, e.StackTrace);
                }
                

                // add to user table
                string query = string.Format("INSERT INTO {0} (payment_id, collection_id, paid, state, amount, paid_amount, due_at, email, mobile, name, url, job_id, paid_at) " +
                    "VALUES (@payment_id, @collection_id, @paid, @state, @amount, @paid_amount, @due_at, @email, @mobile, @name, @url, @job_id, @paid_at);",
                    TABLE_PAYMENTS);

                /*
                string query = string.Format("INSERT INTO {0} (payment_id, collection_id, paid, state, amount, paid_amount, due_at, email, mobile, name, url, job_id, paid_at) " +
                    "VALUES (@payment_id, @collection_id, @paid, @state, @amount, @paid_amount, @due_at, @email, @mobile, @name, @url, @job_id, @paid_at) " +
                    "ON DUPLICATE KEY UPDATE id= LAST_INSERT_ID(id), paid=@paid, state=@state, paid_amount=@paid_amount, paid_at=@paid_at;",
                    TABLE_PAYMENTS); */

                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@payment_id", bill.id);
                mySqlCmd.Parameters.AddWithValue("@collection_id", bill.collection_id);
                mySqlCmd.Parameters.AddWithValue("@paid", bill.paid);
                mySqlCmd.Parameters.AddWithValue("@state", bill.state);
                mySqlCmd.Parameters.AddWithValue("@amount", bill.amount);
                mySqlCmd.Parameters.AddWithValue("@paid_amount", bill.paid_amount);
                mySqlCmd.Parameters.AddWithValue("@due_at", dueDate.ToString("yyyy-MM-dd HH:mm:ss"));
                mySqlCmd.Parameters.AddWithValue("@email", bill.email);
                mySqlCmd.Parameters.AddWithValue("@mobile", bill.mobile == null? "":bill.mobile);
                mySqlCmd.Parameters.AddWithValue("@name", bill.name);
                mySqlCmd.Parameters.AddWithValue("@url", bill.url);
                mySqlCmd.Parameters.AddWithValue("@job_id", jobId);
                mySqlCmd.Parameters.AddWithValue("@paid_at", bill.paid_at);

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

        public Model.BillPlz.Bill GetByJobId(string jobId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT * FROM {0} WHERE job_id=@job_id AND DATE(due_at) > DATE(NOW()) ORDER BY due_at DESC LIMIT 1;",
                    TABLE_PAYMENTS);

                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@job_id", jobId);

                reader = PerformSqlQuery(mySqlCmd);

                if (reader.Read())
                {
                    return new Model.BillPlz.Bill()
                    {
                        reference_1 = reader["job_id"].ToString(),           // this is unencoded jobid
                        url = reader["url"].ToString()
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


        public Model.BillPlz.Bill Get(string paymentId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> queryParams = new Dictionary<string, string>();
                queryParams.Add("payment_id", paymentId);

                mySqlCmd = GenerateQueryCmd(TABLE_PAYMENTS, queryParams);
                reader = PerformSqlQuery(mySqlCmd);
                
                if (reader.Read())
                {
                    return new Model.BillPlz.Bill()
                    {
                        id = reader["payment_id"].ToString(),
                        reference_1 = reader["job_id"].ToString()           // this is unencoded jobid
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
    }
}