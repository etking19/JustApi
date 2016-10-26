using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class OtpController : BaseController
    {
        public Response Put(string userId, string otp, string newPw)
        {
            var lastOtp = otpDao.Get(userId);
            if (lastOtp == null)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EInvalidOtp);
                return response;
            }

            if (lastOtp.otpCode.CompareTo(otp) != 0)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EInvalidOtp);
                return response;
            }

            TimeSpan ts = DateTime.UtcNow - DateTime.Parse(lastOtp.creationDate);
            if (ts.Minutes > 5)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EOtpExpired);
                return response;
            }

            // update the password
            if (false == userDao.UpdatePassword(userId, newPw))
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            // disable the pin
            otpDao.DisableAll(userId);

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
