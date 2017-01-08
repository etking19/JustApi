using JustApi.Constant;
using JustApi.Model.BillPlz;
using JustApi.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class PaymentCallbackController : BaseController
    {
        public void Post([FromBody]Model.BillPlz.Bill bill)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(bill))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(bill);
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, name + ": " + value);
            }

            try
            {
                if (ConfigurationManager.AppSettings.Get("Debug") != "1")
                {
                    // Debug mode, push the notification straight away

                    var debugJobId = Utils.DecodeUniqueId(bill.reference_1);
                    var debugJobDetails = jobDetailsDao.GetByJobId(debugJobId);
                    if (debugJobDetails == null)
                    {
                        DBLogger.GetInstance().Log(DBLogger.ESeverity.Info, string.Format("Debug - Payment callback does not found job details. Job id: {0}, PaymentId: {1}. {2}", debugJobId, bill.id, bill));
                        return;
                    }

                    var extraDataPartner = Helper.PushNotification.ConstructExtraData(Helper.PushNotification.ECategories.NewOpenJob, debugJobId);
                    var partnerListIdentifiers = userDao.GetUserIdentifiersByRoleId(((int)Constants.Configuration.Role.CompanyAdmin).ToString());
                    if (int.Parse(debugJobDetails.jobTypeId) == (int)Constants.Configuration.DeliveryJobType.Standard)
                    {
                        Utility.UtilNotification.BroadCastMessage(
                            partnerListIdentifiers.ToArray(),
                            extraDataPartner,
                            NotificationMsg.NewOpenJob_Title,
                            NotificationMsg.NewOpenJob_Desc + string.Format("From: {0}\nTo: {1}\nAmount:{2}",
                                debugJobDetails.addressFrom[0].address3,
                                debugJobDetails.addressTo[0].address3,
                                debugJobDetails.amountPartner)
                            );
                    }
                    else if (int.Parse(debugJobDetails.jobTypeId) == (int)Constants.Configuration.DeliveryJobType.Disposal)
                    {
                        Utility.UtilNotification.BroadCastMessage(
                            partnerListIdentifiers.ToArray(),
                            extraDataPartner,
                            NotificationMsg.NewOpenJob_Title,
                            NotificationMsg.NewOpenJob_Desc + string.Format("Dispose items from: {0}\nAmount:{1}",
                                debugJobDetails.addressFrom[0].address3,
                                debugJobDetails.amountPartner)
                            );
                    }

                    // do not proceed for debug mode
                    return;
                }
            }
            catch (Exception e)
            {
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Critical, e.Message);
                return;
            }
            


            var jobId = Utils.DecodeUniqueId(bill.reference_1);
            var jobDetails = jobDetailsDao.GetByJobId(jobId);
            if (jobDetails == null)
            {
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Critical, string.Format("Payment callback does not found job details. Job id: {0}, PaymentId: {1}. {2}", jobId, bill.id, bill));
                return;
            }

            // cross check the job id match with exisiting payment id
            var dbBill = paymentsDao.Get(bill.id);
            if (dbBill == null ||
                dbBill.reference_1 != jobId)
            {
                DBLogger.GetInstance().Log(DBLogger.ESeverity.Critical, string.Format("Payment callback does not found matched job id. Job id: {0}, PaymentId: {1}. {2}", jobId, bill.id, bill));
                return;
            }

            // cross check with billplz server status is correct
            var request = WebRequest.Create(string.Format("https://www.billplz.com/api/v3/bills/{0}", bill.id)) as HttpWebRequest;
            using (var responseApi = request.GetResponse() as HttpWebResponse)
            {
                using (var reader = new StreamReader(responseApi.GetResponseStream()))
                {
                    String responseContent = reader.ReadToEnd();
                    Bill jsonObj = JsonConvert.DeserializeObject<Bill>(responseContent);

                    // add to database
                    var id = paymentsDao.AddOrUpdate(bill.reference_1, jsonObj);
                    if (id == null || id == "0")
                    {
                        DBLogger.GetInstance().Log(DBLogger.ESeverity.Critical, string.Format("Payment callback unable to update database. Job id: {0}, PaymentId: {1}. {2}, {3}", jobId, bill.id, bill.due_at));
                        return;
                    }

                    // update the job details on the amount paid
                    var totalPaidAmount = jobDetailsDao.UpdatePaidAmount(jobId, jsonObj.paid_amount);
                    if (totalPaidAmount < jobDetails.amount)
                    {
                        DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, string.Format("Payment callback paid amount not same as total amount. Job id: {0}, PaymentId: {1}. {2}", jobId, bill.id, bill));
                        return;
                    }

                    // update the job order status
                    if (false == jobDeliveryDao.UpdateJobStatus(jobId, ((int)Constants.Configuration.JobStatus.PaymentVerifying).ToString()))
                    {
                        DBLogger.GetInstance().Log(DBLogger.ESeverity.Critical, string.Format("Payment callback unable to update job order status. Job id: {0}, PaymentId: {1}. {2}", jobId, bill.id, bill));
                        return;
                    }

                    // send notification to partners
                    var extraDataPartner = Helper.PushNotification.ConstructExtraData(Helper.PushNotification.ECategories.NewOpenJob, jobId);
                    var partnerListIdentifiers = userDao.GetUserIdentifiersByRoleId(((int)Constants.Configuration.Role.CompanyAdmin).ToString());
                    if (int.Parse(jobDetails.jobTypeId) == (int)Constants.Configuration.DeliveryJobType.Standard)
                    {
                        Utility.UtilNotification.BroadCastMessage(
                            partnerListIdentifiers.ToArray(),
                            extraDataPartner,
                            NotificationMsg.NewOpenJob_Title,
                            NotificationMsg.NewOpenJob_Desc + string.Format("From: {0}\nTo: {1}\nAmount:{2}",
                                jobDetails.addressFrom[0].address3,
                                jobDetails.addressTo[0].address3,
                                jobDetails.amountPartner)
                            );
                    }
                    else if (int.Parse(jobDetails.jobTypeId) == (int)Constants.Configuration.DeliveryJobType.Disposal)
                    {
                        Utility.UtilNotification.BroadCastMessage(
                            partnerListIdentifiers.ToArray(),
                            extraDataPartner,
                            NotificationMsg.NewOpenJob_Title,
                            NotificationMsg.NewOpenJob_Desc + string.Format("Dispose items from: {0}\nAmount:{1}",
                                jobDetails.addressFrom[0].address3,
                                jobDetails.amountPartner)
                            );
                    }

                    // temporary send email to care about the payment
                    var userObj = userDao.GetUserById(jobDetails.ownerUserId);
                    var fleetType = fleetTypeDao.Get(jobDetails.fleetTypeId);
                    var jobType = jobTypeDao.GetById(jobDetails.jobTypeId);
                    UtilEmail.SendOrderReceived(jobDetails.jobId, userObj, jobDetails, fleetType.name, jobType.name);

                }
            }
        }
    }
}
