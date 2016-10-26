using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class LoginController : BaseController
    {
        public Response Post(string username, string password)
        {
            // verify username and password
            User user = userDao.GetUserByUsername(username, true);

            if (user == null)
            {
                // user not found
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EUserNotFound);
                return response;
            }

            if (user.password.CompareTo(password) != 0)
            {
                // invalid user
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.ECredentialError);
                return response;
            }

            if (user.enabled == false)
            {
                // account disabled
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EAccountDisabled);
                return response;
            }

            if (user.deleted)
            {
                // account deleted
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EAccountDeleted);
                return response;
            }

            // get the company role 
            var permissionList = companyDao.GetCompanyPermission(user.userId);
            if (permissionList == null)
            {
                // account deleted
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            if (permissionList.Count == 0)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.ECompanyNotFound);
                return response;
            }

            // 2. update the token info
            string newToken = Guid.NewGuid().ToString();
            string newValidity = commonDao.GetCurrentUtcTime(Constants.Configuration.TOKEN_VALID_HOURS);

            if (false == userDao.InsertOrUpdateToken(user.userId, newToken, newValidity))
            {
                // invalid error
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EUnknownError);
                return response;
            }

            // return user details
            user.password = null;
            Model.Token tokenPayload = new Token()
            {
                user = user,
                token = newToken,
                validTill = newValidity,
                companyList = permissionList
            };

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            response.payload = javaScriptSerializer.Serialize(tokenPayload);
            return response;
        }
    }
}
