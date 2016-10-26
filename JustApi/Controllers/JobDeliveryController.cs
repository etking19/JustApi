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
    public class JobDeliveryController : BaseController
    {
        public Response Get(string limit=null, string skip=null, string jobId=null, string companyId=null,
            string driverId=null, string statusId=null, string uniqueId=null)
        {
            if (jobId != null)
            {
                var result = jobDeliveryDao.Get(jobId);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (uniqueId != null)
            {
                try
                {
                    var decodedJobId = Utils.DecodeUniqueId(uniqueId).ToString();
                    var result = jobDeliveryDao.Get(decodedJobId);
                    if (result == null)
                    {
                        response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                        return response;
                    }

                    response.payload = javaScriptSerializer.Serialize(result);
                }
                catch (Exception)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EParameterError);
                    return response;
                }
            }
            else if (companyId != null)
            {
                var result = jobDeliveryDao.GetByDeliverCompany(companyId, statusId, limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (driverId != null)
            {
                var result = jobDeliveryDao.GetByDriver(driverId, statusId, limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (statusId != null)
            {
                var result = jobDeliveryDao.GetByStatus(statusId, limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else
            {
                var result = jobDeliveryDao.Get(limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Post(string jobId, string companyId, string driverId, string fleetId)
        {
            var result = jobDeliveryDao.Add(jobId, companyId, driverId, fleetId);
            if (result == null)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            // pre-caution step to avoid the job delivery cancelled
            jobDeclineDao.Remove(jobId, companyId);

            // send email to notify user
            JobDetails jobDetails = jobDetailsDao.GetByJobId(jobId);
            User userDetails = userDao.GetUserById(jobDetails.ownerUserId);
            User driverDetails = userDao.GetUserById(driverId);
            Fleet fleetDetails = fleetDao.Get(fleetId);
            JobType jobType = jobTypeDao.GetById(jobDetails.jobTypeId);
            FleetType fleetType = fleetTypeDao.Get(jobDetails.fleetTypeId);

            var uniqueId = Utils.EncodeUniqueId(jobId);
            UtilEmail.SendOrderConfirmed(uniqueId, userDetails, jobDetails, driverDetails, fleetDetails, jobType, fleetType);

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Put(string jobId, string companyId, string driverId, string fleetId)
        {
            if (false == jobDeliveryDao.Update(jobId, companyId, driverId, fleetId))
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Delete(string jobId)
        {
            if (false == jobDeliveryDao.Delete(jobId))
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
