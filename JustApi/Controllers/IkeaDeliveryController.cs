using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class IkeaDeliveryController : BaseController
    {
        public Response Post([FromBody]Ikea ikea)
        {
            // add the user first
            var userId = userDao.AddUser(ikea.user);
            if (null == userId)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            // add the ikea delivery
            var result = ikeaDao.Add(ikea.unitNumber, ikea.project.id, userId, ikea.itemUrl);
            if (null == result)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            // temporary send email to notify admin
            var projectObj = ikeaProjectDao.Get(ikea.project.id);
            Utility.UtilEmail.SendIkeaOrderReceived(ikea.user, ikea.unitNumber, projectObj, ikea.itemUrl, result);

            response.payload = result;
            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Get(string limit = null, string skip = null, string projectId = null, string jobStatusId = null)
        {
            if (projectId != null)
            {
                // get by job id
                var result = ikeaDao.GetByProjectId(projectId, jobStatusId, limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else
            {
                // get by job unique id
                // convert to job id
                var result = ikeaDao.Get(limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
