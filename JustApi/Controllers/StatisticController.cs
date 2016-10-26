using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class StatisticController : BaseController
    {
        public Response Get(string jobStatusId = null, string companyId = null, string driverId = null)
        {
            if (driverId != null)
            {
                var result = statisticDao.GetByUserId(driverId);
                if (null == result)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EResourceNotFoundError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (companyId != null)
            {
                var result = statisticDao.GetByCompany(companyId);
                if (null == result)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EResourceNotFoundError);
                    return response;
                }
                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (jobStatusId != null)
            {
                var result = statisticDao.GetByJobStatus(jobStatusId);
                if (null == result)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EResourceNotFoundError);
                    return response;
                }
                response.payload = javaScriptSerializer.Serialize(result);
            }
            else
            {
                var result = statisticDao.GetAll();
                if (null == result)
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
