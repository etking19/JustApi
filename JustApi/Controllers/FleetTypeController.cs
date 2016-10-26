using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class FleetTypeController : BaseController
    {
        public Response Get([FromUri] string fleetTypeId = null)
        {
            if (fleetTypeId != null)
            {
                var result = fleetTypeDao.Get(fleetTypeId);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EResourceNotFoundError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else
            {
                var result = fleetTypeDao.Get();
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
