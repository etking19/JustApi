using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class BaseDao
    {
        private Constants.Configuration configuration = new Constants.Configuration();

        public string GetCurrentUtcTime(int hourOffset)
        {
            return DateTime.UtcNow.AddHours(hourOffset).ToString("yyyy-MM-dd HH:mm:ss");
        }

        public int RefreshToken(string adminUsername, string newtoken, string newValidity)
        {
            // refresh for another hour
            MySqlCommand tokenCommand = new MySqlCommand("UPDATE users SET validity=@0 and token=@1 where username=@2");
            tokenCommand.Parameters.AddWithValue("@0", newValidity);
            tokenCommand.Parameters.AddWithValue("@1", newtoken);
            tokenCommand.Parameters.AddWithValue("@2", adminUsername);

            return PerformSqlNonQuery(tokenCommand);
        }

        public int RefreshToken(string adminUsername)
        {
            string newValidity = GetCurrentUtcTime(Constants.Configuration.TOKEN_VALID_HOURS);

            // refresh for another hour
            MySqlCommand tokenCommand = new MySqlCommand("UPDATE users SET validity=@0 where username=@1");
            tokenCommand.Parameters.AddWithValue("@0", newValidity);
            tokenCommand.Parameters.AddWithValue("@1", adminUsername);

            return PerformSqlNonQuery(tokenCommand);
        }

        public int PerformSqlNonQuery(MySqlCommand command)
        {
            int result = 0;
            using (MySqlConnection conn = new MySqlConnection(configuration.CONNECTION_STRING))
            {
                conn.Open();
                command.Connection = conn;
                result = command.ExecuteNonQuery();
                conn.Close();
            }

            return result;
        }

        public MySqlDataReader PerformSqlQuery(MySqlCommand command)
        {
            MySqlConnection conn = new MySqlConnection(configuration.CONNECTION_STRING);
            conn.Open();
            command.Connection = conn;
            return command.ExecuteReader();
        }


        public object PerformSqlExeuteScalar(MySqlCommand command)
        {
            MySqlConnection conn = new MySqlConnection(configuration.CONNECTION_STRING);
            conn.Open();
            command.Connection = conn;
            return command.ExecuteScalar();
        }

        public void CleanUp(MySqlDataReader reader, MySqlCommand command)
        {
            try
            {
                reader.Close();
            }
            catch (Exception)
            {
            }

            try
            {
                command.Connection.Close();
            }
            catch (Exception)
            {
            }
        }

        public MySqlCommand GenerateQueryCmd(string tableName)
        {
            string query = string.Format("SELECT * FROM {0}", tableName);
            return new MySqlCommand(query);
        }

        public MySqlCommand GenerateQueryCmdWithLimit(string tableName, string offset, string limit)
        {
            string query = string.Format("SELECT * FROM {0} LIMIT {1} OFFSET {2} ", tableName, limit, offset);
            return new MySqlCommand(query);
        }

        public MySqlCommand GenerateQueryCmdWithLimit(string tableName, Dictionary<string, string> queryParam, string offset, string limit)
        {
            // "SELECT * FROM masteradmins where username=@0 and password=@1"
            string query = string.Format("SELECT * FROM {0} where ", tableName);

            MySqlCommand command = new MySqlCommand();

            int count = 0;
            foreach (KeyValuePair<string, string> entry in queryParam)
            {
                if (count != 0)
                {
                    query += "and ";
                }
                query += string.Format("{0}=@{1} ", entry.Key, count);
                command.Parameters.AddWithValue(string.Format("@{0}", count), entry.Value);

                count++;
            }

            query += string.Format(" LIMIT {0} OFFSET {1};", limit, offset);
            command.CommandText = query;

            return command;
        }

        public MySqlCommand GenerateQueryCmd(string tableName, Dictionary<string, string> queryParam)
        {
            // "SELECT * FROM masteradmins where username=@0 and password=@1"
            string query = string.Format("SELECT * FROM {0} where ", tableName);

            MySqlCommand command = new MySqlCommand();

            int count = 0;
            foreach (KeyValuePair<string, string> entry in queryParam)
            {
                if (count != 0)
                {
                    query += "and ";
                }
                query += string.Format("{0}=@{1} ", entry.Key, count);
                command.Parameters.AddWithValue(string.Format("@{0}", count), entry.Value);

                count++;
            }

            command.CommandText = query;
            return command;
        }

        public MySqlCommand GenerateAddCmd(string tableName, Dictionary<string, string> insertParam)
        {
            MySqlCommand command = new MySqlCommand();

            string parameters = "";
            string values = "";
            int count = 0;
            foreach (KeyValuePair<string, string> entry in insertParam)
            {
                if(entry.Value == null)
                {
                    // skip
                    continue;
                }

                if (count != 0)
                {
                    parameters += ",";
                    values += ",";
                }

                parameters += entry.Key;
                values += string.Format("@{0}", count);
                command.Parameters.AddWithValue(string.Format("@{0}", count), entry.Value);

                count++;
            }


            string query = string.Format("INSERT into {0} ({1}) values ({2});", tableName, parameters, values);
            command.CommandText = query;

            return command;
        }

        public MySqlCommand GenerateEditCmd(string tableName, Dictionary<string, string> updateParam, Dictionary<string, string> destinationParam)
        {
            MySqlCommand command = new MySqlCommand();

            string query = string.Format("UPDATE {0} SET ", tableName);

            int count = 0;
            foreach (KeyValuePair<string, string> entry in updateParam)
            {
                if (entry.Value == null)
                {
                    // skip
                    continue;
                }

                if (count != 0)
                {
                    query += ",";
                }
                query += string.Format("{0}=@{1}", entry.Key, count);
                command.Parameters.AddWithValue(string.Format("@{0}", count), entry.Value);

                count++;
            }

            query += " where ";
            int nextcount = 100;
            foreach (KeyValuePair<string, string> entry in destinationParam)
            {
                if (nextcount != 100)
                {
                    query += " and ";
                }
                query += string.Format("{0}=@{1}", entry.Key, nextcount);
                command.Parameters.AddWithValue(string.Format("@{0}", nextcount), entry.Value);

                nextcount++;
            }

            command.CommandText = query;

            return command;
        }

        public MySqlCommand GenerateRemoveCmd(string tableName, Dictionary<string, string> removeParam)
        {
            string query = string.Format("DELETE from {0} where ", tableName);

            MySqlCommand command = new MySqlCommand();

            int count = 0;
            foreach (KeyValuePair<string, string> entry in removeParam)
            {
                if (count != 0)
                {
                    query += " and ";
                }

                query += string.Format("{0}=@{1}", entry.Key, count);
                command.Parameters.AddWithValue(string.Format("@{0}", count), entry.Value);
                count++;
            }

            command.CommandText = query;

            return command;
        }

        public MySqlCommand GenerateSoftDelete(string tableName, Dictionary<string, string> destinationParam)
        {
            MySqlCommand command = new MySqlCommand();

            string query = string.Format("UPDATE {0} SET deleted=1 WHERE ", tableName);
            int count = 0;
            foreach (KeyValuePair<string, string> entry in destinationParam)
            {
                if (count != 0)
                {
                    query += " and ";
                }
                query += string.Format("{0}=@{1}", entry.Key, count);
                command.Parameters.AddWithValue(string.Format("@{0}", count), entry.Value);

                count++;
            }

            command.CommandText = query;
            return command;
        }
    }
}