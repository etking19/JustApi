using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class ActivityController : BaseController
    {
        public Response Get()
        {
            var result = activityDao.Get();
            if (result == null)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EResourceNotFoundError);
                return response;
            }

            response.payload = javaScriptSerializer.Serialize(result);
            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
