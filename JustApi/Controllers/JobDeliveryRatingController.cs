using JustApi.Constant;
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
    public class JobDeliveryRatingController : BaseController
    {
        public Response Post(string uniqueId, float rating)
        {
            if (rating > 5)
            {
                DBLogger.GetInstance().Log(Utility.DBLogger.ESeverity.Warning, "SetRating, " + uniqueId + "," + rating);
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EParameterError);
                return response;
            }

            var jobId = Utils.DecodeUniqueId(uniqueId);
            if (false == jobDeliveryDao.UpdateRating(jobId, rating))
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            // notify company admin rating update
            var companyAdminsIdentifiers = userDao.GetDeliveryComIdentifierByJobId(jobId, ((int)Constants.Configuration.Role.CompanyAdmin).ToString());

            if (companyAdminsIdentifiers.Count > 0)
            {
                var jobDetails = jobDetailsDao.GetByJobId(jobId);
                var extraData = Helper.PushNotification.ConstructExtraData(Helper.PushNotification.ECategories.RatingUpdate, jobId);

                var description = NotificationMsg.JobRating_Desc.Replace("@rating", rating.ToString());
                description = description.Replace("@jobId", jobId.ToString());
                description = description.Replace("@from", jobDetails.addressFrom[0].address3);

                Utility.UtilNotification.BroadCastMessage(
                    companyAdminsIdentifiers.ToArray(),
                    extraData,
                    NotificationMsg.JobRating_Title,
                    description
                    );
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
