using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Constant
{
    public class ErrorMsg
    {
        private static ErrorMsg sInstance = null;
        private Dictionary<int, string> errorMsgDic; 

        private ErrorMsg()
        {
            // initialize all error value here
            errorMsgDic = new Dictionary<int, string>();

            errorMsgDic.Add(ErrorCode.ESuccess, "Request success.");

            errorMsgDic.Add(ErrorCode.EGeneralError, "General error. Please see administrator to resolve issue.");
            errorMsgDic.Add(ErrorCode.EUnknownError, "Internal error. Please try again later.");
            errorMsgDic.Add(ErrorCode.EParameterError, "API error. Please try again later.");
            errorMsgDic.Add(ErrorCode.EResourceNotFoundError, "Resource requested not found, please try again.");

            errorMsgDic.Add(ErrorCode.ECredentialError, "Username or Password incorrect.");
            errorMsgDic.Add(ErrorCode.ETokenError, "Token was invalid");
            errorMsgDic.Add(ErrorCode.ETokenExpired, "Token expired. Please login again.");
            errorMsgDic.Add(ErrorCode.EMultipleLogin, "You have been logout. Someone login from another device.");
            errorMsgDic.Add(ErrorCode.EAccountDisabled, "You account been disabled. Please see administrator to activate back.");
            errorMsgDic.Add(ErrorCode.EAccountDeleted, "You account been deleted. Please see administrator for more info.");
            errorMsgDic.Add(ErrorCode.EUserNotFound, "User not found. To register please contact administrator.");
            errorMsgDic.Add(ErrorCode.EUserAlreadyExisted, "User has been registered previously. Please try another username.");
            errorMsgDic.Add(ErrorCode.EUserPasswordError, "Old password does not match with system password. Please try again.");
            errorMsgDic.Add(ErrorCode.ERetryTime, "Last one time pin was sent. Please wait until you receive it before retry.");
            errorMsgDic.Add(ErrorCode.EInvalidOtp, "Invalid One Time Pin. Please make sure you key in correct pin and try again.");
            errorMsgDic.Add(ErrorCode.EInvalidRefNum, "Invalid reference number. Please contact administrator for more info.");
            errorMsgDic.Add(ErrorCode.EOtpExpired, "One time pin expired. Please request a new pin and try again.");


            errorMsgDic.Add(ErrorCode.ECompanyDeleted, "Company deleted. Please see administrator for more info.");
            errorMsgDic.Add(ErrorCode.ECompanyNotFound, "Company not found. Please see administrator for more info.");
            errorMsgDic.Add(ErrorCode.ECompanyDisabled, "Company disabled. Please see administrator for more info.");

            errorMsgDic.Add(ErrorCode.EJobDeleted, "Job deleted. Please see administrator for more info.");
            errorMsgDic.Add(ErrorCode.EJobNotFound, "Job not found. Please try different ID.");
            errorMsgDic.Add(ErrorCode.EJobDisabled, "Job disabled. Please see administrator for more info.");

            errorMsgDic.Add(ErrorCode.EPostcodeNotValid, "Postcode entered not valid.");
            errorMsgDic.Add(ErrorCode.EPostcodeFromNotSupport, "Deliver from postcode not supported.");
            errorMsgDic.Add(ErrorCode.EPostcodeToNotSupport, "Deliver to postcode not supported.");
            errorMsgDic.Add(ErrorCode.EVoucherNotValid, "Promo code not valid.");
            errorMsgDic.Add(ErrorCode.EVoucherExpired, "Promo code expired.");
            errorMsgDic.Add(ErrorCode.EVoucherMinimumRequired, "Minimum spending required.");
            errorMsgDic.Add(ErrorCode.EVoucherRedemptionLimit, "All vouchers has been redeem.");
        }

        public static ErrorMsg GetInstance()
        {
            if(sInstance == null)
            {
                sInstance = new ErrorMsg();
            }

            return sInstance;
        }

        public string GetErrorMsg(int errorCode)
        {
            string errorMsg = "";
            errorMsgDic.TryGetValue(errorCode, out errorMsg);

            return errorMsg;
        }
    }
}