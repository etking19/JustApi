using JustApi.Model;
using JustApi.Model.Google;
using JustApi.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class PostcodeController : BaseController
    {
        private string getAddFromLocal(string postcode)
        {
            Postcode postcodeClass = new Postcode();
            string nameLocal;
            var result = postcodeClass.PostcodeNameList.TryGetValue(postcode, out nameLocal);
            if (result)
            {
                return postcode + ", " + nameLocal;
            }

            return "";
        }

        public Response Post(string deliverFrom = null, string deliverTo = null)
        {
            // send check address to google map api service
            try
            {
                string result = "";
                string url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?region=my&address={0}", deliverFrom);
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // by calling .Result you are performing a synchronous call
                        var responseContent = response.Content;

                        // by calling .Result you are synchronously reading the result
                        result = responseContent.ReadAsStringAsync().Result;
                    }
                }

                string fromFormattedAdd = null;
                Model.Google.Bounds fromBound = null; 
                do
                {
                    MapsGeocode jsonObj = JsonConvert.DeserializeObject<MapsGeocode>(result);
                    if (jsonObj.status.CompareTo("OK") != 0)
                    {
                        break;
                    }
                    var representAddFrom = jsonObj.results.Find(t => t.formatted_address.Contains("Malaysia"));
                    if (representAddFrom == null)
                    {
                        break;
                    }

                    fromFormattedAdd = representAddFrom.formatted_address;
                    fromBound = representAddFrom.geometry.bounds;
                    break;

                } while (true);

                if (fromFormattedAdd == null)
                {
                    // fail to get from Google, use local
                    fromFormattedAdd = getAddFromLocal(deliverFrom);
                }

                if (fromFormattedAdd == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EPostcodeNotValid);
                    return response;
                }

                /*
                 * Temporary removed the checking
                 *
                // validate the available postcode
                string[] supportedFrom = supportedAreaDao.GetFrom();
                if (Utils.ContainsAny(fromFormattedAdd, supportedFrom) == false)
                {
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Analytic, string.Format("From Not Supported: {0}", fromFormattedAdd));
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EPostcodeFromNotSupport);
                    return response;
                }
                */

                string toFormattedAdd = null;
                Model.Google.Bounds toBound = null;
                if (deliverTo != null)
                {
                    url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?region=my&address={0}", deliverTo);
                    using (var client = new HttpClient())
                    {
                        var response = client.GetAsync(url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            // by calling .Result you are performing a synchronous call
                            var responseContent = response.Content;

                            // by calling .Result you are synchronously reading the result
                            result = responseContent.ReadAsStringAsync().Result;
                        }
                    }

                    do
                    {
                        var jsonObj = JsonConvert.DeserializeObject<MapsGeocode>(result);
                        if (jsonObj.status.CompareTo("OK") != 0)
                        {
                            break;
                        }

                        var representAddTo = jsonObj.results.Find(t => t.formatted_address.Contains("Malaysia"));
                        if (representAddTo == null)
                        {
                            break;
                        }

                        toFormattedAdd = representAddTo.formatted_address;
                        toBound = representAddTo.geometry.bounds;
                        break;

                    } while (true);


                    if (toFormattedAdd == null)
                    {
                        // fail to get from Google, use local
                        toFormattedAdd = getAddFromLocal(deliverTo);
                    }

                    if (toFormattedAdd == null)
                    {
                        response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EPostcodeNotValid);
                        return response;
                    }

                    /*
                     * Temporary remove the checking 
                     * 
                    // validate the available postcode
                    string[] supportedTo = supportedAreaDao.GetTo();
                    if (Utils.ContainsAny(toFormattedAdd, supportedTo) == false)
                    {
                        DBLogger.GetInstance().Log(DBLogger.ESeverity.Analytic, string.Format("To Not Supported: {0}", toFormattedAdd));
                        response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EPostcodeToNotSupport);
                        return response;
                    }
                    */
                }

                DBLogger.GetInstance().Log(DBLogger.ESeverity.Analytic, string.Format("From Interested: {0}", fromFormattedAdd));
                PostcodeList postCodeList = new PostcodeList()
                {
                    postCodeAddFrom = fromFormattedAdd
                };

                if (toFormattedAdd != null)
                {
                    postCodeList.postCodeAddTo = toFormattedAdd;
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Analytic, string.Format("To Interested: {0}", toFormattedAdd));
                }

                // calculate distance between postcode only for standard delivery
                if (fromFormattedAdd != null &&
                    toFormattedAdd != null)
                {
                    url = string.Format("https://maps.googleapis.com/maps/api/directions/json?region=my&origin={0}&destination={1}&mode=driving", fromFormattedAdd, toFormattedAdd);
                    using (var client = new HttpClient())
                    {
                        var response = client.GetAsync(url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            // by calling .Result you are performing a synchronous call
                            var responseContent = response.Content;

                            // by calling .Result you are synchronously reading the result
                            result = responseContent.ReadAsStringAsync().Result;
                        }
                    }

                    Direction directionObj = JsonConvert.DeserializeObject<Direction>(result);
                    if (directionObj.status.CompareTo("OK") != 0)
                    {
                        response.payload = javaScriptSerializer.Serialize(directionObj.status);
                    }

                    postCodeList.fromBound = fromBound;
                    postCodeList.toBound = toBound;
                    postCodeList.distance = directionObj.routes[0].legs[0].distance.value;
                    postCodeList.duration = directionObj.routes[0].legs[0].duration.value;
                }

                response.payload = javaScriptSerializer.Serialize(postCodeList);
            }
            catch (Exception)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
