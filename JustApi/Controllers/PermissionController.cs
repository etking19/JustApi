using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class PermissionController : BaseController
    {
        public Response Get()
        {
            var result = permissionDao.Get();
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
