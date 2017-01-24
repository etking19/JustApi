using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class UserController : BaseController
    {
        public Response Post([FromBody] User user)
        {
            // check if user add with role 
            string password = null;
            if (user.roleId != null &&
                user.password == null)
            {
                // generate random password for this user
                Random generator = new Random();
                password = generator.Next(0, 1000000).ToString("D6");

                user.password = password;
            }

            var result = userDao.AddUser(user);
            if (result == null)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            // notify user with the latest password created
            if (password != null)
            {
                Utility.UtilSms.SendSms(user.contactNumber, string.Format("From JustLorry. Your account has been created. Please download JustPartner from Android or iOS app store. Username: {0}, Password: {1}", user.username, password));
            }


            response.payload = result;
            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Put(string userId, [FromBody] User user)
        {
            if (false == userDao.UpdateUser(userId, user))
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Delete(string userId)
        {
            if (false == userDao.DeleteUser(userId))
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Get(string limit = null, string skip = null, string userId = null, 
            string username = null, string companyId = null, string roleId = null)
        {
            if (userId != null)
            {
                // return single user id
                var result = userDao.GetUserById(userId);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EUserNotFound);
                    return response;
                }

                if (result.deleted)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EAccountDeleted);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (username != null)
            {
                // get user by username
                var result = userDao.GetUserByUsername(username);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EUserNotFound);
                    return response;
                }

                if (result.deleted)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EAccountDeleted);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (companyId != null)
            {
                // get user by company id
                var result = userDao.GetUserByCompanyId(companyId, roleId, limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EResourceNotFoundError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else
            {
                // get all users
                var result = userDao.GetAllUsers(limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EResourceNotFoundError);
                    return response;
                }
                response.payload = javaScriptSerializer.Serialize(result);
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
