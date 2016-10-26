using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class FleetController : BaseController
    {
        public Response Post([FromBody] Fleet fleet)
        {
            var result = fleetDao.Add(fleet);
            if (result == null)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response.payload = result;
            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Put(string id, [FromBody] Fleet fleet)
        {
            if (fleetDao.Update(id, fleet) == false)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Get(string limit = null, string skip = null, string fleetId = null, string companyId = null)
        {
            if (fleetId != null)
            {
                var result = fleetDao.Get(fleetId);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EResourceNotFoundError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (companyId != null)
            {
                var result = fleetDao.GetByCompanyId(companyId, limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EResourceNotFoundError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else
            {
                var result = fleetDao.Get(limit, skip);
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

        public Response Delete(string id)
        {
            if (fleetDao.Delete(id) == false)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
