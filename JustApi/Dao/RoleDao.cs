using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Model;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class RoleDao : BaseDao
    {
        private readonly string TABLE_ROLES = "roles";
        private readonly string TABLE_ACTIVITY_PERMISSION = "role_activity_permission";
        private readonly string TABLE_ACTIVITIES = "activities";
        private readonly string TABLE_PERMISSIONS = "permissions";

        public Role GetRoleById(string roleId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> queryParam = new Dictionary<string, string>();
                queryParam.Add("id", roleId);

                mySqlCmd = GenerateQueryCmd(TABLE_ROLES, queryParam);
                reader = PerformSqlQuery(mySqlCmd);
                if(reader.Read())
                {
                    return new Role()
                    {
                        roleId = reader["id"].ToString(),
                        name = reader["name"].ToString()
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

        public List<Role> GetRoles()
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                mySqlCmd = GenerateQueryCmd(TABLE_ROLES);
                reader = PerformSqlQuery(mySqlCmd);

                List<Role> roleList = new List<Role>();
                while (reader.Read())
                {
                    roleList.Add(new Role()
                    {
                        roleId = reader["id"].ToString(),
                        name = reader["name"].ToString()
                    });
                }

                return roleList;
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

        public Dictionary<Permission, Activity> GetPermissionsFromRole(string roleId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT {1}.id as Aid, {1}.name as Aname, {2}.id as Pid, {2}.name as Pname FROM {0} " +
                    "INNER JOIN {1} ON {0}.activity_id={1}.id " +
                    "INNER JOIN {2} ON {0}.permission_id={2}.id " +
                    "WHERE role_id={3};",
                    TABLE_ACTIVITY_PERMISSION, TABLE_ACTIVITIES, TABLE_PERMISSIONS, roleId);

                mySqlCmd = new MySqlCommand(query);
                reader = PerformSqlQuery(mySqlCmd);

                Dictionary<Permission, Activity> permissionList = new Dictionary<Permission, Activity>();
                while (reader.Read())
                {
                    permissionList.Add(
                        new Permission()
                        {
                            permissionId = reader["Pid"].ToString(),
                            name = reader["Pname"].ToString()
                        }, 
                        new Activity
                        {
                            activityId = reader["Aid"].ToString(),
                            name = reader["Aname"].ToString()
                        }
                    );
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