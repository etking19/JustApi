using JustApi.Constant;
using JustApi.Model;
using JustApi.Model.Google;
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
    public class JobController : BaseController
    {
        public Response Get(string jobId = null, string uniqueId = null, string ownerId = null,
            string jobTypeId = null, string fromDate = null, string toDate = null, string limit = null, string skip = null)
        {
            if (jobId != null)
            {
                // get by job id
                var result = jobDetailsDao.GetByJobId(jobId);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (uniqueId != null)
            {
                // get by job unique id
                // convert to job id
                var decodedJobId = Utils.DecodeUniqueId(uniqueId);
                var result = jobDetailsDao.GetByJobId(decodedJobId);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (ownerId != null)
            {
                // get by creator id
                var result = jobDetailsDao.GetByOwnerId(ownerId, fromDate, toDate, limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (jobTypeId != null)
            {
                var result = jobDetailsDao.GetByJobTypeId(jobTypeId, fromDate, toDate, limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else if (fromDate != null ||
                toDate != null)
            {
                var result = jobDetailsDao.GetByDateRange(fromDate, toDate, limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else
            {
                // get all
                var result = jobDetailsDao.Get(limit, skip);
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

        public Response Post([FromBody]Model.JobDetails jobDetails, string promoCode = null)
        {
            try
            {
                // first add the user if not existed
                var userId = jobDetails.ownerUserId;
                var userObj = userDao.GetUserById(userId);
                if (userObj == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EParameterError);
                    return response;
                }

                // validate the voucher
                if (promoCode != null)
                {
                    var voucherResult = new Vouchers();

                    // TODO: bug here: as the amount pass in was discounted amount, so it might below the expected use amount
                    // best way is not taking the amount required, and recalculate here to avoid injection attack
                    var responseCode = validateVoucher(promoCode, jobDetails.amount, out voucherResult);
                    if (responseCode != Constant.ErrorCode.ESuccess)
                    {
                        response = Utility.Utils.SetResponse(response, false, responseCode);
                        return response;
                    }

                    if (voucherDao.IncreaseUsedCount(promoCode) == false)
                    {
                        DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, "voucherDao.IncreaseUsedCount(promoCode) in Common controller: " + promoCode);
                        response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EVoucherNotValid);
                        return response;
                    }
                }
                 
                // get the gps coordinate if not passed in
                // get the state id and country id if not passed in
                foreach (Model.Address address in jobDetails.addressFrom)
                {
                    if (address.gpsLongitude == 0 ||
                        address.gpsLatitude == 0 ||
                        address.stateId == null ||
                        address.countryId == null)
                    {
                        // request gps cordinate
                        AddressComponents mapsObj = Utils.GetGpsCoordinate(address.address1, address.address2, address.address3, address.postcode);
                        if (mapsObj == null)
                        {
                            // find from local database
                            Postcode postcodeClass = new Postcode();
                            string nameLocal;
                            var result = postcodeClass.PostcodeNameList.TryGetValue(address.postcode, out nameLocal);
                            if (result == false)
                            {
                                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                                return response;
                            }
                            mapsObj = Utils.GetGpsCoordinate(nameLocal);
                        }

                        if (address.gpsLongitude == 0)
                        {
                            address.gpsLongitude = mapsObj.geometry.location.lng;
                        }

                        if (address.gpsLatitude == 0)
                        {
                            address.gpsLatitude = mapsObj.geometry.location.lat;
                        }

                        if (address.countryId == null)
                        {
                            var countryObj = countryDao.GetCountries().Find(t => t.name.Contains(mapsObj.address_components.Find(c => c.types.Contains("country")).long_name));
                            address.countryId = countryObj.countryId;
                        }

                        if (address.stateId == null)
                        {
                            var stateList = stateDao.GetByCountryId(address.countryId);
                            try
                            {
                                var stateObj = stateList.Find(t => t.name.Contains(mapsObj.address_components.Find(a => a.types.Contains("administrative_area_level_1")).long_name));
                                if (stateObj == null)
                                {
                                    // cannot find from google api, use local database
                                    Postcode postcodeClass = new Postcode();
                                    string stateLocal;
                                    var localDic = postcodeClass.PostcodeList.TryGetValue(address.postcode, out stateLocal);
                                    address.stateId = stateList.Find(t => t.name.Contains(stateLocal)).stateId;
                                }
                                else
                                {
                                    address.stateId = stateObj.stateId;
                                }
                            }
                            catch (Exception)
                            {
                                // cannot find from google api, use local database
                                Postcode postcodeClass = new Postcode();
                                string stateLocal;
                                var localDic = postcodeClass.PostcodeList.TryGetValue(address.postcode, out stateLocal);
                                address.stateId = stateList.Find(t => t.name.Contains(stateLocal)).stateId;
                            }

                        }
                    }
                }

                if (jobDetails.addressTo == null)
                {
                    jobDetails.addressTo = new List<Model.Address>();
                }

                foreach (Model.Address address in jobDetails.addressTo)
                {
                    if (address.gpsLongitude == 0 ||
                        address.gpsLatitude == 0 ||
                        address.stateId == null ||
                        address.countryId == null)
                    {
                        // request gps cordinate
                        AddressComponents mapsObj = Utils.GetGpsCoordinate(address.address1, address.address2, address.address3, address.postcode);
                        if (mapsObj == null)
                        {
                            // find from local database
                            Postcode postcodeClass = new Postcode();
                            string nameLocal;
                            var result = postcodeClass.PostcodeNameList.TryGetValue(address.postcode, out nameLocal);
                            if (result == false)
                            {
                                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                                return response;
                            }
                            mapsObj = Utils.GetGpsCoordinate(nameLocal);
                        }

                        if (address.gpsLongitude == 0)
                        {
                            address.gpsLongitude = mapsObj.geometry.location.lng;
                        }

                        if (address.gpsLatitude == 0)
                        {
                            address.gpsLatitude = mapsObj.geometry.location.lat;
                        }

                        if (address.countryId == null)
                        {
                            var countryObj = countryDao.GetCountries().Find(t => t.name.Contains(mapsObj.address_components.Find(c => c.types.Contains("country")).long_name));
                            address.countryId = countryObj.countryId;
                        }

                        if (address.stateId == null)
                        {
                            var stateList = stateDao.GetByCountryId(address.countryId);

                            try
                            {
                                var stateObj = stateList.Find(t => t.name.Contains(mapsObj.address_components.Find(a => a.types.Contains("administrative_area_level_1")).long_name));
                                if (stateObj == null)
                                {
                                    // cannot find from google api, use local database
                                    Postcode postcodeClass = new Postcode();
                                    string stateLocal;
                                    postcodeClass.PostcodeList.TryGetValue(address.postcode, out stateLocal);
                                    address.stateId = stateList.Find(t => t.name.Contains(stateLocal)).stateId;
                                }
                                else
                                {
                                    address.stateId = stateObj.stateId;
                                }
                            }
                            catch (Exception)
                            {
                                // cannot find from google api, use local database
                                Postcode postcodeClass = new Postcode();
                                string stateLocal;
                                postcodeClass.PostcodeList.TryGetValue(address.postcode, out stateLocal);
                                address.stateId = stateList.Find(t => t.name.Contains(stateLocal)).stateId;
                            }
                        }
                    }
                }

                // handle if partner amount is not present
                if (jobDetails.amountPartner == 0)
                {
                    if (jobDetails.jobTypeId == ((int)JustApi.Constants.Configuration.DeliveryJobType.Standard).ToString())
                    {
                        // standard delivery

                        // find the partner price for distance in this distance
                        StandardDeliveryController tempController = new StandardDeliveryController();
                        var priceDetails = tempController.GetPrice(jobDetails.distance.ToString(), jobDetails.fleetTypeId,
                            jobDetails.addressFrom[0].buildingType, jobDetails.addressTo[0].buildingType, jobDetails.workerAssistant.ToString(),
                            jobDetails.assembleBed.ToString(), jobDetails.assembleDiningTable.ToString(), jobDetails.assembleWardrobe.ToString(), 
                            jobDetails.assembleOfficeTable.ToString(),
                            jobDetails.bubbleWrapping.ToString(), jobDetails.shrinkWrapping.ToString());

                        jobDetails.amountPartner = priceDetails.partnerTotal;
                    }
                    else if(jobDetails.jobTypeId == ((int)JustApi.Constants.Configuration.DeliveryJobType.Disposal).ToString())
                    {
                        // disposal
                        DisposalDeliveryController tempController = new DisposalDeliveryController();
                        var priceDetails = tempController.GetPrice(jobDetails.fleetTypeId, jobDetails.addressFrom[0].buildingType, promoCode);

                        jobDetails.amountPartner = priceDetails.partnerTotal;
                    }
                }

                // add the job details
                jobDetails.createdBy = userId;
                jobDetails.modifiedBy = userId;
                var jobId = jobDetailsDao.Add(jobDetails);
                if (jobId == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                // add the job status
                if (null == jobDetailsDao.AddOrder(jobId, userId))
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                // add the address from, to
                foreach (Model.Address add in jobDetails.addressFrom)
                {
                    add.createdBy = userId;
                    var result = addressDao.Add(add, jobId, userObj.displayName, userObj.contactNumber, Dao.AddressDao.EType.From);
                    if (result == null)
                    {
                        response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                        return response;
                    }
                }

                foreach (Model.Address add in jobDetails.addressTo)
                {
                    add.createdBy = userId;
                    var result = addressDao.Add(add, jobId, userObj.displayName, userObj.contactNumber, Dao.AddressDao.EType.To);
                    if (result == null)
                    {
                        response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                        return response;
                    }
                }

                // generate the unique job id
                var uniqueId = Utils.EncodeUniqueId(jobId);

                // request the job payment
                PaymentController controller = new PaymentController();
                var paymentReq = controller.Post(uniqueId);

                // send notification to creator
                var clientIdentifiers = userDao.GetDeviceIdentifier(userId);
                var msg = NotificationMsg.NewJob_Desc + uniqueId;
                if (clientIdentifiers != null &&
                    clientIdentifiers.Count != 0)
                {
                    // user have app installed and identifier found, send push notification
                    var extraData = Helper.PushNotification.ConstructExtraData(Helper.PushNotification.ECategories.OrderCreated, uniqueId);
                    Utility.UtilNotification.BroadCastMessage(clientIdentifiers.ToArray(), extraData, NotificationMsg.NewJob_Title, msg);
                }

                if (ConfigurationManager.AppSettings.Get("Debug") != "1")
                {
                    // send sms together because no history of push notification
                    UtilSms.SendSms(userObj.contactNumber, msg);
                }

                // send email to user
                var fleetType = fleetTypeDao.Get(jobDetails.fleetTypeId);
                var jobType = jobTypeDao.Get().Find(t => t.jobTypeId == jobDetails.jobTypeId);
                UtilEmail.SendInvoice(uniqueId, (string)paymentReq.payload, userObj, jobDetails, fleetType.name, jobType.name);

                response.payload = uniqueId;
                response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);

                return response;
            }
            catch (Exception e)
            {
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Error, e.Message);
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, e.StackTrace);

                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EUnknownError);
                return response;
            }
            
        }

        public Response Delete(string jobId)
        {
            if (jobDetailsDao.Delete(jobId))
            {
                response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
                return response;
            }

            response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
            return response;
        }

        public Response Put(string jobId, [FromBody]JobDetails jobDetails)
        {
            if (jobId == null ||
                jobDetails == null)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EParameterError);
                return response;
            }

            // update the job base on input
            jobDetails.jobId = jobId;
            if (false == jobDetailsDao.Update(jobDetails))
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            // TODO: update the address



            // TODO: inform job delivery company and driver if any changes


            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
