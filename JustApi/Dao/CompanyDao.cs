using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JustApi.Utility;

namespace JustApi.Dao
{
    public class CompanyDao : BaseDao
    {
        private readonly string TABLE_COMPANIES = "companies";

        public string AddCompany(Model.Company company)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                // add to job order status
                Dictionary<string, string> insertParam = new Dictionary<string, string>();
                insertParam.Add("name", company.name);
                insertParam.Add("address_1", company.address1);
                insertParam.Add("address_2", company.address2);
                insertParam.Add("postcode", company.postcode);
                insertParam.Add("state_id", company.stateId);
                insertParam.Add("country_id", company.countryId);
                insertParam.Add("registration_number", company.registrationNumber);

                mySqlCmd = GenerateAddCmd(TABLE_COMPANIES, insertParam);
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

        public Model.Company GetCompanyById(string companyId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> queryParam = new Dictionary<string, string>();
                queryParam.Add("id", companyId);

                mySqlCmd = GenerateQueryCmd(TABLE_COMPANIES, queryParam);
                reader = PerformSqlQuery(mySqlCmd);

                if (false == reader.Read())
                {
                    return null;
                }

                var result = constructObj(reader);

                // query the admin
                var admins = new UsersDao().GetUserByCompanyId(companyId, "2", null, null);  // fixed Company-admin to 2
                if(admins != null)
                {
                    result.admin = admins.ToArray();
                }

                // query the rating
                result.rating = new JobDeliveryDao().GetRatingByCompany(companyId);

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

        public List<Model.Company> GetCompanies(string limit, string skip)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT * FROM {0} WHERE deleted=0 ORDER BY creation_date DESC ", 
                    TABLE_COMPANIES);

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
                List<Model.Company> companyList = new List<Model.Company>();
                while(reader.Read())
                {
                    companyList.Add(constructObj(reader));
                }

                return companyList;
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

        private Model.Company constructObj(MySqlDataReader reader)
        {
            return new Model.Company()
            {
                companyId = reader["id"].ToString(),
                name = reader["name"].ToString(),
                address1 = reader["address_1"].ToString(),
                address2 = reader["address_2"].ToString(),
                postcode = reader["postcode"].ToString(),
                stateId = reader["state_id"].ToString(),
                countryId = reader["country_id"].ToString(),
                registrationNumber = reader["registration_number"].ToString(),
                enabled = (int)reader["enabled"] == 0 ? false : true,
                deleted = (int)reader["deleted"] == 0 ? false : true,
                creationDate = reader["creation_date"].ToString(),
                lastModifiedDate = reader["last_modified_date"].ToString()
            };
        }

        public bool UpdateCompany(string companyId, Model.Company company)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string, string> updateParam = new Dictionary<string, string>();
                updateParam.Add("name", company.name);
                updateParam.Add("address_1", company.address1);
                updateParam.Add("address_2", company.address2);
                updateParam.Add("postcode", company.postcode);
                updateParam.Add("state_id", company.stateId);
                updateParam.Add("country_id", company.countryId);
                updateParam.Add("registration_number", company.registrationNumber);

                Dictionary<string, string> destinationParam = new Dictionary<string, string>();
                destinationParam.Add("id", companyId);

                mySqlCmd = GenerateEditCmd(TABLE_COMPANIES, updateParam, destinationParam);
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

        public bool DeleteCompany(string companyId)
        {
            MySqlCommand mySqlCmd = null;
            try
            {
                Dictionary<string, string> removeParams = new Dictionary<string, string>();
                removeParams.Add("id", companyId);

                mySqlCmd = GenerateSoftDelete(TABLE_COMPANIES, removeParams);
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

        public List<Model.Company> GetCompanyPermission(string userId)
        {
            MySqlCommand mySqlCmd = null;
            MySqlDataReader reader = null;
            try
            {
                string query = string.Format("SELECT rating, {1}.*, {8}.id as Rid, {8}.name as Rname, {4}.id as Aid, {4}.name as Aname, {5}.id as Pid, {5}.name as Pname FROM {0} " +
                    "INNER JOIN {1} ON {0}.company_id={1}.id AND {1}.deleted=0 AND {1}.enabled=1 " +
                    "INNER JOIN {2} ON {0}.user_id={2}.user_id AND {2}.company_id={0}.company_id " +
                    "INNER JOIN {3} ON {2}.role_id={3}.role_id " +
                    "INNER JOIN {4} ON {4}.id={3}.activity_id " +
                    "INNER JOIN {5} ON {5}.id={3}.permission_id " +
                    "INNER JOIN {8} ON {8}.id={3}.role_id " +
                    "LEFT JOIN (SELECT {6}.company_id, AVG(rating) as rating FROM {6}) jobDelivery ON jobDelivery.company_id={0}.company_id " + 
                    "WHERE {0}.user_id={7}",
                    "user_company", "companies", "user_role", "role_activity_permission", "activities", "permissions", "job_delivery", userId, "roles");

                mySqlCmd = new MySqlCommand(query);
                reader = PerformSqlQuery(mySqlCmd);
                List<Model.Company> result = new List<Model.Company>();

                while (reader.Read())
                {
                    var companyId = reader["id"].ToString();
                    var previousData = result.Find(t => t.companyId.CompareTo(companyId) == 0);
                    if (previousData == null)
                    {
                        // new company data
                        previousData = new Model.Company()
                        {
                            companyId = companyId.ToString(),
                            name = reader["name"].ToString(),
                            address1 = reader["address_1"].ToString(),
                            address2 = reader["address_2"].ToString(),
                            postcode = reader["postcode"].ToString(),
                            stateId = reader["state_id"].ToString(),
                            countryId = reader["country_id"].ToString(),
                            registrationNumber = reader["registration_number"].ToString(),
                            enabled = (int)reader["enabled"] == 0 ? false : true,
                            rating = String.IsNullOrEmpty(reader["rating"].ToString()) ? 0 : reader.GetFloat("rating"),
                            rolePermissionList = new List<Model.Role>()
                        };

                        result.Add(previousData);
                    }

                    var roleId = reader["Rid"].ToString();
                    var rolePermissionList = previousData.rolePermissionList.Find(t => t.roleId.CompareTo(roleId) == 0);
                    if (rolePermissionList == null)
                    {
                        rolePermissionList = new Model.Role()
                        {
                            roleId = reader["Rid"].ToString(),
                            name = reader["Rname"].ToString(),
                            permissionList = new List<Model.Permission>()
                        };

                        previousData.rolePermissionList.Add(rolePermissionList);
                    }

                    var permissionId = reader["Pid"].ToString();
                    var permissionList = rolePermissionList.permissionList.Find(t => t.permissionId.CompareTo(permissionId) == 0);
                    if (permissionList == null)
                    {
                        permissionList = new Model.Permission()
                        {
                            permissionId = reader["Pid"].ToString(),
                            name = reader["Pname"].ToString(),
                            activityList = new List<Model.Activity>()
                        };

                        rolePermissionList.permissionList.Add(permissionList);
                    }

                    permissionList.activityList.Add(new Model.Activity()
                    {
                        activityId = reader["Aid"].ToString(),
                        name = reader["Aname"].ToString()
                    });
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
    }
}