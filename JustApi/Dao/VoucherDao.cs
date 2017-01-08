using JustApi.Model;
using JustApi.Utility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace JustApi.Dao
{
    public class VoucherDao : BaseDao
    {
        private readonly string TABLE_VOUCHER = "vouchers";
        private readonly string TABLE_VOUCHER_TYPE = "voucher_type";

        public bool IncreaseUsedCount(string voucherCode)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("UPDATE {0} SET used = used+1 " +
                "WHERE code=@code;",
                TABLE_VOUCHER, TABLE_VOUCHER_TYPE);

                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@code", voucherCode);
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

        public Vouchers GetByVoucherCode(string voucherCode)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT {0}.*,{1}.id as vid,{1}.name as vname FROM {0} " +
                "INNER JOIN {1} ON {0}.voucher_type_id ={1}.id " +
                "WHERE {0}.code=@code;",
                TABLE_VOUCHER, TABLE_VOUCHER_TYPE);

                mySqlCmd = new MySqlCommand(query);
                mySqlCmd.Parameters.AddWithValue("@code", voucherCode);

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

        public List<Vouchers> Get(string limit, string skip)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT {0}.*,{1}.id as vid,{1}.name as vname FROM {0} " +
                "INNER JOIN {1} ON {0}.voucher_type_id ={1}.id " +
                "ORDER BY creation_date ASC ",
                TABLE_VOUCHER, TABLE_VOUCHER_TYPE);

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

                var result = new List<Vouchers>();
                while (reader.Read())
                {
                    result.Add(constructObj(reader));
                }

                return result;
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


        private Vouchers constructObj(MySqlDataReader reader)
        {
            return new Vouchers()
            {
                id = reader["id"].ToString(),
                name = reader["id"].ToString(),
                code = reader["code"].ToString(),
                startDate = reader["start_date"].ToString(),
                endDate = reader["end_date"].ToString(),
                creationDate = reader["creation_date"].ToString(),
                discountValue = reader.GetFloat("discount_value"),
                minimumPurchase = reader.GetFloat("minimum_purchase"),
                maximumDiscount = reader.GetFloat("maximum_discount"),
                quantity = reader.GetInt32("quantity"),
                used = reader.GetInt32("used"),
                enabled = reader.GetInt32("enabled") == 0 ? false : true,
                voucherType = new VoucherType()
                {
                    id = reader["vid"].ToString(),
                    name = reader["vname"].ToString()
                }
            };

        }

        public List<string> GenerateVouchers(int numberOfVouchers, string namePrefix, float minPurchase, float percentageDiscount, DateTime expiredDate, int maxChars)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("insert into {0} (name, end_date, voucher_type_id, discount_value, minimum_purchase, code, quantity) values ",
                    TABLE_VOUCHER);

                List<string> promoCodeList = new List<string>();
                for (int count = 1; count <= numberOfVouchers; count++)
                {
                    string promoCode = Guid.NewGuid().ToString().Substring(1, maxChars);
                    promoCodeList.Add(promoCode);

                    query += string.Format("('{0}-{1}', '{2}', {3}, {4}, {5}, '{6}', 1),",
                        namePrefix, count, expiredDate.ToString("yyyy-MM-dd hh:mm:ss"), 1, percentageDiscount, minPurchase, promoCode);
                }

                // remove the last comma
                query = query.Substring(0, query.Length - 1);

                mySqlCmd = new MySqlCommand(query);
                if(0 != PerformSqlNonQuery(mySqlCmd))
                {
                    return promoCodeList;
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