using JustApi.Model;
using JustApi.Model.BillPlz;
using JustApi.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class PaymentController : BaseController
    {
        public Response Post(string orderId)
        {
            try
            {
                // get info about the order
                var jobId = Utils.DecodeUniqueId(orderId);

                // get from existing database to see if record existed
                var previousResult = paymentsDao.GetByJobId(jobId);
                if (previousResult != null)
                {
                    response.payload = previousResult.url;
                    response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
                    return response;
                }

                var jobDetails = jobDetailsDao.GetByJobId(jobId);
                if (jobDetails == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EParameterError);
                    return response;
                }

                var ownerDetails = userDao.GetUserById(jobDetails.ownerUserId);
                if (ownerDetails == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                // send billplz request
                var request = WebRequest.Create("https://www.billplz.com/api/v3/bills") as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json";

                var usernameEncoded = Utils.Base64Encode(System.Configuration.ConfigurationManager.AppSettings["BillPlzApi"]);
                request.Headers.Add("authorization", "Basic " + usernameEncoded + ":");

                DateTime date = DateTime.Parse(jobDetails.deliveryDate);
                var paymentDue = date.ToString("yyyy-MM-dd");

                var obj = new
                {
                    collection_id = System.Configuration.ConfigurationManager.AppSettings["BillPlzCollectionId"],
                    description = string.Format("Booking id: {0}. Created on: {1}", orderId, jobDetails.creationDate),
                    email = ownerDetails.email,
                    // mobile = ownerDetails.contactNumber, /* Not include because need the format defined +6xxxxx or 60xxxx ONLY */
                    name = ownerDetails.displayName,
                    due_at = paymentDue,
                    amount = jobDetails.amount * 100,
                    callback_url = System.Configuration.ConfigurationManager.AppSettings["BillPlzCallbackUrl"],
                    reference_1_label = "orderId",
                    reference_1 = orderId,
                    reference_2_label = "mobile",
                    reference_2 = ownerDetails.contactNumber
                };

                var param = javaScriptSerializer.Serialize(obj);
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(param);

                string responseContent = null;
                try
                {
                    using (var writer = request.GetRequestStream())
                    {
                        writer.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var responseApi = request.GetResponse() as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(responseApi.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                            Bill jsonObj = JsonConvert.DeserializeObject<Bill>(responseContent);

                            // add to database
                            paymentsDao.AddOrUpdate(jobId, jsonObj);

                            response.payload = jsonObj.url;
                            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
                            return response;
                        }
                    }
                }
                catch (WebException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(new StreamReader(e.Response.GetResponseStream()).ReadToEnd());

                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Error, e.Message);
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, e.StackTrace);
                }
            }
            catch (Exception e)
            {
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Error, e.Message);
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, e.StackTrace);
            }


            response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
            return response;
        }
    }
}
