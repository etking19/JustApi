using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class UtilsController : BaseController
    {
        public Response Post(int total, string prefix, float minSpend, float rate, DateTime expired, int maxChars)
        {
            // generate 20% min rm500 spend
            var result = voucherDao.GenerateVouchers(total, prefix, minSpend, rate, expired, maxChars);

            if (result != null)
            {
                response.payload = result;
                response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
                return response;
            }

            return response;
        }
    }
}
