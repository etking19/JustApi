using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class VoucherController : BaseController
    {
        public Response Post(string promoCode)
        {
            var result = voucherDao.GetByVoucherCode(promoCode);
            if (result == null ||
                result.enabled == false)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EVoucherNotValid);
                return response;
            }

            DateTime startDate = DateTime.Parse(result.startDate);
            DateTime endDate = DateTime.Parse(result.endDate);

            if (DateTime.Now.CompareTo(startDate) < 0 ||
                DateTime.Now.CompareTo(endDate) > 0)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EVoucherExpired);
                return response;
            }

            if (result.used >= result.quantity)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EVoucherRedemptionLimit);
                return response;
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
