using JustApi.Model;
using JustApi.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class PasswordController : BaseController
    {
        public Response Put(string userId, string oldPw, string newPw)
        {
            var userData = userDao.GetUserById(userId);
            if (userData.password.CompareTo(oldPw) != 0)
            {
                // old password not match
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EUserPasswordError);
                return response;
            }

            if (false == userDao.UpdatePassword(userId, newPw))
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Post(string userId)
        {
            // check if the previous OTP is within time limit
            var lastOtp = otpDao.Get(userId);
            if (lastOtp != null)
            {
                TimeSpan ts = DateTime.UtcNow - DateTime.Parse(lastOtp.creationDate);
                if (ts.TotalMinutes < 5)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.ERetryTime);
                    return response;
                }
            }

            // disable all the previous OTP
            if (false == otpDao.DisableAll(userId))
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            // add new otp to system
            string newOtp = new Random().Next(100000, 999999).ToString();
            string newRefNum = Guid.NewGuid().ToString();
            if (false == otpDao.Add(userId, newOtp, newRefNum))
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            // send to user's handphone
            var userData = userDao.GetUserById(userId);
            var responseMsg = string.Format("Just Supply Chain Berhad.%0AYour OTP is: {0}. This OTP valid for 5 minutes.", newOtp);
            UtilSms.SendSms(userData.contactNumber, responseMsg);

            // TODO: generate email and send to user's email

            response.success = true;
            response.errorCode = Constant.ErrorCode.ESuccess;
            response.errorMessage = "Your temporary password was sent to your registered mobile phone.";

            return response;
        }
    }
}
