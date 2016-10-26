using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class PermissionDao : BaseDao
    {
        private readonly string TABLE_PERMISSION = "permissions";

        public List<Model.Permission> Get()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                mySqlCmd = GenerateQueryCmd(TABLE_PERMISSION);
                reader = PerformSqlQuery(mySqlCmd);

                List<Model.Permission> permissionList = new List<Model.Permission>();
                while (reader.Read())
                {
                    permissionList.Add(new Model.Permission()
                    {
                        permissionId = reader["id"].ToString(),
                        name = reader["name"].ToString()
                    });
                }

                return permissionList;
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