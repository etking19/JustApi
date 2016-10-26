using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class JobTypeController : BaseController
    {
        public Response Get([FromUri] string jobTypeId = null)
        {
            if (jobTypeId != null)
            {
                var result = jobTypeDao.GetById(jobTypeId);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EResourceNotFoundError);
                    return response;
                }
                response.payload = javaScriptSerializer.Serialize(result);
            }
            else
            {
                var result = jobTypeDao.Get();
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
