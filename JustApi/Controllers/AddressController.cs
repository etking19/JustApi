using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class AddressController : BaseController
    {
        public Response Get(string limit=null, string skip=null, string userId=null, string from=null, string to=null)
        {
            if (userId == null ||
                (from == null && to == null))
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EParameterError);
                return response;
            }

            var type = from != null ? Dao.AddressDao.EType.From : Dao.AddressDao.EType.To;
            var result = addressDao.Get(userId, limit, skip, type);

            if (result == null)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response.payload = javaScriptSerializer.Serialize(result);
            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
