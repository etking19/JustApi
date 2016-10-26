using JustApi.Constant;
using JustApi.Model;
using JustApi.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class JobDeliveryStatusController : BaseController
    {
        public Response Get(string uniqueId = null, string jobId = null)
        {
            if (uniqueId != null)
            {
                jobId = Utils.DecodeUniqueId(uniqueId);
            }

            if (jobId == null)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EParameterError);
                return response;
            }

            var result = jobDetailsDao.GetJobStatus(jobId);

            if (result == null)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EJobNotFound);
                return response;
            }

            if (result.deleted)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EJobDeleted);
                return response;
            }

            if (result.enabled == false)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EJobDisabled);
                return response;
            }

            response.payload = javaScriptSerializer.Serialize(result);
            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Put(string jobId, string statusId, string pickupErrId = null, string deliverErrId = null)
        {
            var result = jobDeliveryDao.UpdateJobStatus(jobId, statusId, pickupErrId, deliverErrId);
            if (false == result)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            // notify owner status update
            var jobDetails = jobDetailsDao.GetByJobId(jobId);
            var clientIdentifiers = userDao.GetDeviceIdentifier(jobDetails.ownerUserId);
            var uniqueId = Utils.EncodeUniqueId(jobId);
            var msg = NotificationMsg.JobStatusUpdate_Desc + uniqueId;
            if (clientIdentifiers != null)
            {
                // user have app installed and identifier found, send push notification
                var extraData = Helper.PushNotification.ConstructExtraData(Helper.PushNotification.ECategories.OrderStatusUpdate, uniqueId);
                Utility.UtilNotification.BroadCastMessage(clientIdentifiers.ToArray(), extraData, NotificationMsg.NewJob_Title, msg);
            }
            /* do not use SMS for job status update
            else
            {
                // no device record, send sms instead
                var userObj = userDao.GetUserById(jobDetails.ownerUserId);
                UtilSms.SendSms(userObj.contactNumber, msg);
            }
            */

            if (statusId == ((int)Constants.Configuration.JobStatus.Delivered).ToString())
            {
                // send email to notify user
                User userDetails = userDao.GetUserById(jobDetails.ownerUserId);
                UtilEmail.SendDelivered(userDetails.email, uniqueId, userDetails.displayName,
                    ConfigurationManager.AppSettings.Get("RatingLink") + "?uniqueId=" + uniqueId);
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
